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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Transmitly.ChannelProvider.Twilio.Configuration;
using Transmitly.Delivery;
using Transmitly.Util;
using Twilio.Rest.Api.V2010.Account;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Sms
{
	public sealed class TwilioSmsChannelProviderDispatcher(TwilioClientOptions twilioClientOptions) : ChannelProviderRestDispatcher<ISms>(null)
	{
		private readonly TwilioClientOptions _twilioClientOptions = Guard.AgainstNull(twilioClientOptions);

		protected override async Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(HttpClient restClient, ISms sms, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
		{
			Guard.AgainstNull(sms);
			Guard.AgainstNull(communicationContext);

			var recipients = communicationContext.PlatformIdentities.SelectMany(a => a.Addresses.Select(addr => addr.Value)).ToList();
			var smsProperties = new ExtendedSmsChannelProperties(sms.ExtendedProperties);
			var results = new List<IDispatchResult>(recipients.Count);

			foreach (var recipient in recipients)
			{
				Dispatch(communicationContext, sms);

				var message = await MessageResource.CreateAsync(new CreateMessageOptions(recipient)
				{
					From = string.IsNullOrWhiteSpace(smsProperties.MessagingServiceSid) ? sms.From?.Value : null,
					Body = sms.Message,
					MessagingServiceSid = smsProperties.MessagingServiceSid,
					StatusCallback = await GetStatusCallbackUrl(smsProperties, sms, communicationContext).ConfigureAwait(false)
				}, new TwilioHttpClient(restClient, _twilioClientOptions)).ConfigureAwait(false);

				var isDispatched = IsDispatched(message);
				var twResult = new TwilioDispatchResult(message.Sid, isDispatched, message.Status.ToString());

				results.Add(twResult);

				if (isDispatched)
					Dispatched(communicationContext, sms, [twResult]);
				else
					Error(communicationContext, sms, [twResult]);
			}

			return results;
		}

		private static async Task<Uri?> GetStatusCallbackUrl(ExtendedSmsChannelProperties smsProperties, ISms sms, IDispatchCommunicationContext context)
		{
			string? url = smsProperties.StatusCallbackUrl;

			var resolveUrl = smsProperties.StatusCallbackUrlResolver ?? sms.DeliveryReportCallbackUrlResolver;
			if (resolveUrl != null)
			{
				var urlResult = await resolveUrl(context).ConfigureAwait(false);
				if (string.IsNullOrWhiteSpace(urlResult))
					return null;
				return new Uri(urlResult);
			}

			if (string.IsNullOrWhiteSpace(url))
				return null;

			return new Uri(url).AddPipelineContext(string.Empty, context.PipelineIntent, context.PipelineId, context.ChannelId, context.ChannelProviderId);
		}

		protected override void ConfigureHttpClient(HttpClient httpClient)
		{
			RestClientConfiguration.Configure(httpClient, _twilioClientOptions);
			base.ConfigureHttpClient(httpClient);
		}

		private static bool IsDispatched(MessageResource resource)
		{
			return resource.Status != MessageResource.StatusEnum.Failed && resource.Status != MessageResource.StatusEnum.Undelivered;
		}


	}
}