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

using Transmitly.ChannelProvider.Twilio.Configuration;
using Transmitly.ChannelProvider.Twilio.Configuration.Voice;
using Transmitly.ChannelProvider.Twilio.Sdk.Sms;
using Transmitly.ChannelProvider.Twilio.Sdk.Voice;
using Transmitly.Delivery;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Tests
{
	[TestClass]
	public sealed class DeliveryStatusTests
	{
		[TestMethod]
		public async Task SmsCallback_WithoutSmsSidOrSmsStatus_UsesMessageSidAndMessageStatus()
		{
			var report = await AdaptAsync(new() { ["MessageSid"] = "SM1", ["MessageStatus"] = "delivered" });

			Assert.AreEqual("SM1", report.ResourceId);
			Assert.IsTrue(report.Status.IsSuccess());
			Assert.AreEqual("Delivered", report.Status.Type);
		}

		[TestMethod]
		public async Task SmsCallback_WithOnlySmsStatus_StillWorks()
		{
			var report = await AdaptAsync(new() { ["SmsSid"] = "SM1", ["SmsStatus"] = "undelivered" });

			Assert.AreEqual("SM1", report.ResourceId);
			Assert.IsTrue(report.Status.IsFailure());
		}

		[TestMethod]
		[DataRow("sms", "MessageStatus", "brand_new_status")]
		[DataRow("sms", "MessageStatus", null)]
		[DataRow("voice", "CallStatus", "brand-new-status")]
		[DataRow("voice", "CallStatus", null)]
		public async Task UnknownOrMissingStatus_IsReportedAsUnknown(string channel, string field, string? value)
		{
			var form = new Dictionary<string, string> { ["MessageSid"] = "SM1", ["CallSid"] = "CA1" };
			if (value != null)
				form[field] = value;

			var report = await AdaptAsync(form, channel);

			Assert.IsTrue(report.Status.IsClientError());
			Assert.AreEqual("Unknown", report.Status.Type);
		}

		[TestMethod]
		[DataRow("inbound", Direction.Inbound)]
		[DataRow("outbound-api", Direction.OutboundApi)]
		public async Task VoiceCallback_Direction_IsKept(string value, Direction expected)
		{
			// Inbound is the enum's first member, so it used to be mistaken for "missing" and dropped.
			var report = await AdaptAsync(new() { ["CallSid"] = "CA1", ["CallStatus"] = "completed", ["Direction"] = value }, "voice");

			Assert.AreEqual(expected, report.Twilio().Voice.Direction);
		}

		private static async Task<DeliveryReport> AdaptAsync(Dictionary<string, string> form, string channel = "sms")
		{
			var options = new TwilioClientOptions { ValidateDeliveryReportSignature = false };
			IChannelProviderDeliveryReportRequestAdaptor adaptor = channel == "sms"
				? new TwilioSmsDeliveryStatusReportAdaptor(options)
				: new TwilioVoiceDeliveryStatusReportAdaptor(options);

			return (await adaptor.AdaptAsync(new FormRequest(form, channel)))!.Single();
		}
	}
}
