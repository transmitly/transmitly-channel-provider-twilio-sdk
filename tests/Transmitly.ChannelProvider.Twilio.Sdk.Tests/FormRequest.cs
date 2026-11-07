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

using System.Net;
using Transmitly.Delivery;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Tests
{
	/// <summary>
	/// An unsigned Twilio status callback (form post) addressed to the given channel.
	/// </summary>
	internal sealed class FormRequest(Dictionary<string, string> form, string channel = "sms") : IRequestAdaptorContext
	{
		public string? GetQueryValue(string key) => key == DeliveryUtil.ChannelIdKey ? channel : key == DeliveryUtil.ChannelProviderIdKey ? "twilio" : null;
		public string? GetFormValue(string key) => form.TryGetValue(key, out var v) ? v : null;
		public string? GetHeaderValue(string key) => null;
		public string? GetValue(string key) => throw new System.NotSupportedException("The ASP.NET hosts throw here; use GetQueryValue, GetFormValue or GetHeaderValue.");
		public string? Content { get; } = string.Join("&", form.Select(kv => WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value)));
		public string? RequestUrl => $"https://hooks.example.com/delivery?tlyc={channel}&tlycp=twilio";
		public string? PipelineIntent => null;
		public string? PipelineId => null;
		public string? ResourceId => null;
	}
}
