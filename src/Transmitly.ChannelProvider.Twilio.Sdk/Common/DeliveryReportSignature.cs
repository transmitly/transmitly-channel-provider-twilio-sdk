// Copyright (c) Code Impressions, LLC. All Rights Reserved.
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
using System.Collections.Specialized;
using System.Net;
using Transmitly.ChannelProvider.Twilio.Configuration;
using Transmitly.Delivery;
using Twilio.Security;

namespace Transmitly.ChannelProvider.Twilio.Sdk
{
	// https://www.twilio.com/docs/usage/webhooks/webhooks-security
	internal static class DeliveryReportSignature
	{
		private const string SignatureHeader = "X-Twilio-Signature";

		public static void Validate(IRequestAdaptorContext context, TwilioClientOptions options)
		{
			if (!options.ValidateDeliveryReportSignature)
				return;
			if (string.IsNullOrWhiteSpace(options.AuthToken))
				throw new DeliveryReportRequestValidationException("Twilio AuthToken is required to validate status callbacks.");

			var url = GetUrl(context.RequestUrl, options.DeliveryReportBaseUrl);
			var signature = context.GetHeaderValue(SignatureHeader);
			if (url == null || string.IsNullOrEmpty(signature) || !new RequestValidator(options.AuthToken).Validate(url, ParseForm(context.Content), signature))
				throw new DeliveryReportRequestValidationException("Twilio status callback signature is invalid.");
		}

		private static string? GetUrl(string? requestUrl, string? baseUrl)
		{
			if (requestUrl == null || string.IsNullOrWhiteSpace(baseUrl))
				return requestUrl;
			var schemeEnd = requestUrl.IndexOf("://", StringComparison.Ordinal);
			var pathStart = schemeEnd < 0 ? -1 : requestUrl.IndexOf('/', schemeEnd + 3);
			return baseUrl!.TrimEnd('/') + (pathStart < 0 ? string.Empty : requestUrl[pathStart..]);
		}

		// Twilio signs the URL plus every POSTed form field.
		private static NameValueCollection ParseForm(string? body)
		{
			var form = new NameValueCollection();
			foreach (var pair in (body ?? string.Empty).Split('&'))
			{
				if (pair.Length == 0)
					continue;
				var i = pair.IndexOf('=');
				form.Add(WebUtility.UrlDecode(i < 0 ? pair : pair[..i]), i < 0 ? string.Empty : WebUtility.UrlDecode(pair[(i + 1)..]));
			}
			return form;
		}
	}
}
