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

using System.Net.Http;
using System.Threading.Tasks;
using Transmitly.ChannelProvider.Twilio.Configuration;
using TWC = Twilio.Clients;
using TWH = Twilio.Http;

namespace Transmitly.ChannelProvider.Twilio.Sdk
{
	sealed class TwilioHttpClient(HttpClient client, TwilioClientOptions twilioClientOptions) : TWC.ITwilioRestClient
	{
		public string AccountSid => _client.AccountSid;

		public string Region => _client.Region;

		private readonly TWC.TwilioRestClient _client = new(
				twilioClientOptions.AccountSid,
				twilioClientOptions.AuthToken,
				twilioClientOptions.AccountSid,
				twilioClientOptions.Region,
				new TWH.SystemNetHttpClient(client),
				twilioClientOptions.Edge
			);

		public TWH.HttpClient HttpClient => _client.HttpClient;

		public TWH.Response Request(TWH.Request request)
		{
			return _client.Request(request);
		}

		public Task<TWH.Response> RequestAsync(TWH.Request request)
		{
			return _client.RequestAsync(request);
		}
	}
}