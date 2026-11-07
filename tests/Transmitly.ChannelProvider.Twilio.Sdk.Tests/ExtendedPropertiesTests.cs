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
using Moq;
using Transmitly.Channel.Configuration.Sms;
using Transmitly.Channel.Configuration.Voice;
using Transmitly.ChannelProvider.Twilio.Configuration;
using Transmitly.ChannelProvider.Twilio.Configuration.Sms;
using Transmitly.ChannelProvider.Twilio.Sdk.Sms;
using Transmitly.Delivery;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Tests
{
	/// <summary>
	/// Twilio values live on the channel or report itself, so nothing has to be registered for them to be kept.
	/// </summary>
	[TestClass]
	public sealed class ExtendedPropertiesTests
	{
		[TestMethod]
		public void SmsSettings_SetOnTheChannel_AreReadFromTheDispatchedSms()
		{
			// Core hands the channel's extended properties to the sms it builds for dispatch.
			IExtendedProperties bag = new ExtendedProperties();
			var channel = Mock.Of<ISmsChannelConfiguration>(c => c.ExtendedProperties == bag);
			channel.Twilio().MessagingServiceSid = "MG1";
			channel.Twilio().StatusCallbackUrl = "https://hooks.example.com/sms";

			var sms = Mock.Of<ISms>(s => s.ExtendedProperties == bag).Twilio();

			Assert.AreEqual("MG1", sms.MessagingServiceSid);
			Assert.AreEqual("https://hooks.example.com/sms", sms.StatusCallbackUrl);
		}

		[TestMethod]
		public void VoiceMachineDetection_IsNullUntilSet()
		{
			// An unset Twilio value must not override the machine detection set on the voice channel itself.
			IExtendedProperties bag = new ExtendedProperties();
			var channel = Mock.Of<IVoiceChannelConfiguration>(c => c.ExtendedProperties == bag);
			Assert.IsNull(channel.Twilio().MachineDetection);

			channel.Twilio().MachineDetection = MachineDetection.Enabled;

			Assert.AreEqual(MachineDetection.Enabled, Mock.Of<IVoice>(v => v.ExtendedProperties == bag).Twilio().MachineDetection);
		}

		[TestMethod]
		public async Task SmsStatusCallback_IsReadableFromTheReport()
		{
			var adaptor = new TwilioSmsDeliveryStatusReportAdaptor(new TwilioClientOptions { ValidateDeliveryReportSignature = false });
			var form = new Dictionary<string, string>
			{
				["MessageSid"] = "SM1",
				["SmsSid"] = "SM1",
				["MessageStatus"] = "delivered",
				["SmsStatus"] = "delivered",
				["To"] = "+15555550123",
				["From"] = "+15555550100",
				["AccountSid"] = "AC1"
			};

			var report = (await adaptor.AdaptAsync(new FormRequest(form)))!.Single();

			var sms = report.Twilio().Sms;
			Assert.AreEqual("SM1", sms.MessageSid);
			Assert.AreEqual(SmsStatus.Delivered, sms.MessageStatus);
			Assert.AreEqual("+15555550123", sms.To);
			Assert.AreEqual("AC1", sms.AccountSid);
			Assert.AreEqual("+15555550123", ((ISmsDeliveryReport)report).To);
		}
	}
}
