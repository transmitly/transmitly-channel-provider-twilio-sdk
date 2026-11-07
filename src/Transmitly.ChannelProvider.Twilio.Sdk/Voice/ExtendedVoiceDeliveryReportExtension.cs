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

using Transmitly.ChannelProvider.Twilio.Configuration.Voice;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Voice
{
	static class ExtendedVoiceDeliveryReportExtension
	{
		public static VoiceDeliveryReport ApplyExtendedProperties(this VoiceDeliveryReport voiceDeliveryReport, VoiceStatusReport report)
		{
			var voice = voiceDeliveryReport.Twilio().Voice;
			voice.To = report.To;
			voice.From = report.From;
			voice.AccountSid = report.AccountSid;
			voice.ApiVersion = report.ApiVersion;
			voice.CallSid = report.CallSid;
			voice.Duration = report.Duration;
			voice.Timestamp = report.Timestamp;
			voice.AnsweredBy = report.AnsweredBy;
			voice.CallbackSource = report.CallbackSource;
			voice.IdempotencyId = report.IdempotencyId;
			voice.FromState = report.FromState;
			voice.FromZip = report.FromZip;
			voice.CalledState = report.CalledState;
			voice.CallDuration = report.CallDuration;
			voice.Called = report.Called;
			voice.CalledCity = report.CalledCity;
			voice.CalledCountry = report.CalledCountry;
			voice.CalledZip = report.CalledZip;
			voice.Caller = report.Caller;
			voice.CallerCity = report.CallerCity;
			voice.CallerCountry = report.CallerCountry;
			voice.CallerState = report.CallerState;
			voice.CallerZip = report.CallerZip;
			voice.CallStatus = report.CallStatus;
			voice.Direction = report.Direction;
			voice.FromCity = report.FromCity;
			voice.FromCountry = report.FromCountry;
			voice.SequenceNumber = report.SequenceNumber;
			voice.Signature = report.Signature;
			voice.SipResponseCode = report.SipResponseCode;
			voice.ToCity = report.ToCity;
			voice.ToCountry = report.ToCountry;
			voice.ToState = report.ToState;
			voice.ToZip = report.ToZip;
			return voiceDeliveryReport;
		}
	}
}