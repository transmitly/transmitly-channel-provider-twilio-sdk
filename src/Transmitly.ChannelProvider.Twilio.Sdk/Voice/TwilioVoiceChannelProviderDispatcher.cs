// ﻿﻿Copyright (c) Code Impressions, LLC. All Rights Reserved.
//  
//  Licensed under the Apache License, Version 2.0 (the "License")
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using TWML = Twilio.TwiML;
using TWT = Twilio.Types;
using TWH = Twilio.Http;
using Transmitly.Delivery;
using Transmitly.Util;
using Transmitly.ChannelProvider.Twilio.Configuration;


namespace Transmitly.ChannelProvider.Twilio.Sdk.Voice
{
	public sealed class TwilioVoiceChannelProviderDispatcher(TwilioClientOptions twilioClientOptions) : ChannelProviderRestDispatcher<IVoice>(null)
	{
		private readonly TwilioClientOptions _twilioClientOptions = twilioClientOptions;

		/// <summary>
		/// Dispatches a Voice communication using Twilio.
		/// </summary>
		/// <param name="restClient">Rest client.</param>
		/// <param name="voice">Voice communication.</param>
		/// <param name="communicationContext">Context of the dispatch.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Result of the dispatch.</returns>
		protected override async Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(System.Net.Http.HttpClient restClient, IVoice voice, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
		{
			var voiceProperties = new ExtendedVoiceChannelProperties(voice.ExtendedProperties);
			var recipients = communicationContext.PlatformIdentities.SelectMany(a => a.Addresses.Select(addr => new TWT.PhoneNumber(addr.Value))).ToList();
			var results = new List<IDispatchResult>(recipients.Count);
			var from = new TWT.PhoneNumber(Guard.AgainstNull(voice.From).Value);

			foreach (var recipient in recipients)
			{
				Dispatch(communicationContext, voice);

				var (twiml, url) = await GetMessageContent(voiceProperties, communicationContext, voice, cancellationToken).ConfigureAwait(false);

				var resource = await CallResource.CreateAsync(new CreateCallOptions(recipient, from)
				{
					Url = url,
					Twiml = twiml,
					Timeout = voiceProperties.Timeout,
					StatusCallback = await GetStatusCallbackUrl(voiceProperties, voice, communicationContext).ConfigureAwait(false),
					StatusCallbackMethod = ConvertHttpMethod(voiceProperties.StatusCallbackMethod),
					MachineDetection = ConvertMachineDetection(voice.MachineDetection, voiceProperties.MachineDetection),
				}, new TwilioHttpClient(restClient, _twilioClientOptions)).ConfigureAwait(false);

				var isDispatched = IsDispatched(resource);
				var twResult = new TwilioDispatchResult(resource.Sid, isDispatched, resource.Status.ToString());

				results.Add(twResult);

				if (isDispatched)
					Dispatched(communicationContext, voice, [twResult]);
				else
					Error(communicationContext, voice, [twResult]);
			}

			return results;
		}

		private static async Task<(string? twiml, Uri? url)> GetMessageContent(ExtendedVoiceChannelProperties voiceProperties, IDispatchCommunicationContext communicationContext, IVoice voice, CancellationToken cancellationToken)
		{
			var messageNeededId = Guid.NewGuid().ToString("N");
			var url = await GetTwiMLCallbackUrl(messageNeededId, voiceProperties, communicationContext).ConfigureAwait(false);
			string? twiml = new TWML.VoiceResponse().Say(voice.Message).ToString();

			if (string.IsNullOrWhiteSpace(twiml) && url == null)
			{
				throw new TwilioClientException("You must specify a Message or a Url.");
			}
			if (twiml == null || twiml.Length > 4000)
			{
				if (url == null)
					throw new TwilioClientException("Body is > 4000 characters. Url is required.");
				twiml = null;
			}
			else if (!string.IsNullOrWhiteSpace(twiml) && twiml.Length < 4000)
			{
				url = null;
			}

			if (url != null && voiceProperties.OnStoreMessageForRetrievalAsync != null)
				_ = Task.Run(() => voiceProperties.OnStoreMessageForRetrievalAsync(messageNeededId, voice, communicationContext).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);

			return (twiml, url);
		}

		private static bool IsDispatched(CallResource resource)
		{
			return resource.Status != CallResource.StatusEnum.Failed && resource.Status != CallResource.StatusEnum.Canceled;
		}
		private static TWH.HttpMethod? ConvertHttpMethod(string? httpMethod)
		{
			return (httpMethod?.ToUpperInvariant()) switch
			{
				"GET" => TWH.HttpMethod.Get,
				"POST" => TWH.HttpMethod.Post,
				"DELETE" => TWH.HttpMethod.Delete,
				"PUT" => TWH.HttpMethod.Put,
				_ => null,
			};
		}

		private static string? ConvertMachineDetection(MachineDetection channelValue, MachineDetection? configuredValue)
		{
			return Enum.GetName(typeof(MachineDetection), configuredValue ?? channelValue);
		}

		private static async Task<Uri?> GetTwiMLCallbackUrl(string messageNeededId, ExtendedVoiceChannelProperties voiceProperties, IDispatchCommunicationContext context)
		{
			//const string RequiredUrlExceptionMessage = $"Twilio requires a url that returns TwiML when called. Ensure you have a value set for the Twilio extended property: '{nameof(voiceProperties.Url)}' OR '{nameof(voiceProperties.UrlResolver)}'.";

			//if (string.IsNullOrWhiteSpace(voiceProperties.Url) && voiceProperties.UrlResolver == null)
			//	throw new TwilioException(RequiredUrlExceptionMessage);

			string? url = voiceProperties.Url;

			if (voiceProperties.UrlResolver != null)
				url = await voiceProperties.UrlResolver(context).ConfigureAwait(false);

			if (string.IsNullOrWhiteSpace(url))
				return null;
			//throw new TwilioException(RequiredUrlExceptionMessage);

			return new Uri(url).AddParameter("messageId", messageNeededId).AddPipelineContext(string.Empty, context.PipelineIntent, context.PipelineId, context.ChannelId, context.ChannelProviderId);
		}

		private static async Task<Uri?> GetStatusCallbackUrl(ExtendedVoiceChannelProperties voiceProperties, IVoice voice, IDispatchCommunicationContext context)
		{
			string? url;
			var urlResolver = voiceProperties.StatusCallbackUrlResolver ?? voice.DeliveryReportCallbackUrlResolver;
			if (urlResolver != null)
				url = await urlResolver(context).ConfigureAwait(false);
			else
				url = voiceProperties.StatusCallbackUrl;

			if (string.IsNullOrWhiteSpace(url))
				return null;

			return new Uri(url).AddPipelineContext(string.Empty, context.PipelineIntent, context.PipelineId, context.ChannelId, context.ChannelProviderId);
		}

		protected override void ConfigureHttpClient(System.Net.Http.HttpClient httpClient)
		{
			RestClientConfiguration.Configure(httpClient, _twilioClientOptions);
			base.ConfigureHttpClient(httpClient);
		}

	}
}