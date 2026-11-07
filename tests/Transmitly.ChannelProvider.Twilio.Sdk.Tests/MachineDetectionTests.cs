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

using Transmitly.ChannelProvider.Twilio.Sdk.Voice;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Tests
{
	[TestClass]
	public sealed class MachineDetectionTests
	{
		[TestMethod]
		[DataRow(MachineDetection.Enabled, "Enable")]
		[DataRow(MachineDetection.MessageEnd, "DetectMessageEnd")]
		[DataRow(MachineDetection.Disabled, null)]
		public void ChannelValue_IsSentAsTwilioDocumentsIt(MachineDetection value, string? expected)
		{
			// The same options the dispatcher builds; GetParams is the form Twilio receives.
			var options = new CreateCallOptions(new PhoneNumber("+15555550123"), new PhoneNumber("+15555550100"))
			{
				MachineDetection = TwilioVoiceChannelProviderDispatcher.ConvertMachineDetection(value, null)
			};

			var sent = options.GetParams().Where(p => p.Key == "MachineDetection").Select(p => p.Value).SingleOrDefault();

			Assert.AreEqual(expected, sent);
		}

		[TestMethod]
		public void TwilioSetting_OverridesTheChannel()
		{
			Assert.IsNull(TwilioVoiceChannelProviderDispatcher.ConvertMachineDetection(MachineDetection.Enabled, MachineDetection.Disabled));
			Assert.AreEqual("DetectMessageEnd", TwilioVoiceChannelProviderDispatcher.ConvertMachineDetection(MachineDetection.Disabled, MachineDetection.MessageEnd));
		}
	}
}
