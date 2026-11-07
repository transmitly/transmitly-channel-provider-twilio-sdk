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
using System.Security.Cryptography;
using System.Text;
using Transmitly.ChannelProvider.Twilio.Configuration;
using Transmitly.ChannelProvider.Twilio.Sdk.Sms;
using Transmitly.ChannelProvider.Twilio.Sdk.Voice;
using Transmitly.Delivery;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Tests
{
	[TestClass]
	public sealed class DeliveryReportSignatureTests
	{
		private const string AuthToken = "test-auth-token";
		private const string PublicUrl = "https://hooks.example.com/delivery?tlyc=sms&tlycp=twilio";
		private static readonly Dictionary<string, string> Form = new() { ["MessageSid"] = "SM1", ["SmsSid"] = "SM1", ["SmsStatus"] = "delivered", ["To"] = "+15555550123" };

		[TestMethod]
		public async Task ValidSignature_ReturnsReport()
		{
			var reports = await Sms().AdaptAsync(Context(PublicUrl, Sign(PublicUrl, Form)));

			Assert.AreEqual("SM1", reports!.Single().ResourceId);
		}

		[TestMethod]
		public async Task TamperedField_Throws()
		{
			var context = Context(PublicUrl, Sign(PublicUrl, Form), new Dictionary<string, string>(Form) { ["SmsStatus"] = "failed" });

			await Assert.ThrowsExactlyAsync<DeliveryReportRequestValidationException>(() => Sms().AdaptAsync(context));
			await Assert.ThrowsExactlyAsync<DeliveryReportRequestValidationException>(() => new TwilioVoiceDeliveryStatusReportAdaptor(Options()).AdaptAsync(Context(PublicUrl.Replace("=sms", "=voice"), "bad")));
		}

		[TestMethod]
		public async Task BehindProxy_UsesConfiguredBaseUrl()
		{
			var serverSees = "http://10.0.0.5:8080/delivery?tlyc=sms&tlycp=twilio";
			var context = Context(serverSees, Sign(PublicUrl, Form));

			await Assert.ThrowsExactlyAsync<DeliveryReportRequestValidationException>(() => Sms().AdaptAsync(context));
			Assert.IsNotNull(await Sms(o => o.DeliveryReportBaseUrl = "https://hooks.example.com/").AdaptAsync(context));
		}

		[TestMethod]
		public async Task NotMine_ReturnsNull_MissingToken_Throws_Disabled_Skips()
		{
			Assert.IsNull(await Sms().AdaptAsync(Context("https://hooks.example.com/delivery?tlyc=email&tlycp=twilio", "bad")));
			await Assert.ThrowsExactlyAsync<DeliveryReportRequestValidationException>(() => Sms(o => o.AuthToken = null).AdaptAsync(Context(PublicUrl, Sign(PublicUrl, Form))));
			Assert.IsNotNull(await Sms(o => o.ValidateDeliveryReportSignature = false).AdaptAsync(Context(PublicUrl, null)));
		}

		private static TwilioClientOptions Options(Action<TwilioClientOptions>? configure = null)
		{
			var options = new TwilioClientOptions { AuthToken = AuthToken };
			configure?.Invoke(options);
			return options;
		}

		private static TwilioSmsDeliveryStatusReportAdaptor Sms(Action<TwilioClientOptions>? configure = null) => new(Options(configure));

		// Twilio: base64(HMAC-SHA1(authToken, url + each POST field name+value, sorted by name)).
		private static string Sign(string url, Dictionary<string, string> form)
		{
			var data = url + string.Concat(form.OrderBy(kv => kv.Key, StringComparer.Ordinal).Select(kv => kv.Key + kv.Value));
			using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(AuthToken));
			return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
		}

		private static FakeRequest Context(string url, string? signature, Dictionary<string, string>? form = null) => new(url, signature, form ?? Form);

		private sealed class FakeRequest(string url, string? signature, Dictionary<string, string> form) : IRequestAdaptorContext
		{
			private readonly Dictionary<string, string> _query = new Uri(url).Query.TrimStart('?').Split('&').Select(p => p.Split('=')).ToDictionary(p => p[0], p => p[1]);

			public string? GetQueryValue(string key) => _query.TryGetValue(key, out var v) ? v : null;
			public string? GetFormValue(string key) => form.TryGetValue(key, out var v) ? v : null;
			public string? GetHeaderValue(string key) => key.Equals("X-Twilio-Signature", StringComparison.OrdinalIgnoreCase) ? signature : null;
			public string? GetValue(string key) => throw new System.NotSupportedException("The ASP.NET hosts throw here; use GetQueryValue, GetFormValue or GetHeaderValue.");
			public string? Content { get; } = string.Join("&", form.Select(kv => WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value)));
			public string? RequestUrl => url;
			public string? PipelineIntent => null;
			public string? PipelineId => null;
			public string? ResourceId => null;
		}
	}
}
