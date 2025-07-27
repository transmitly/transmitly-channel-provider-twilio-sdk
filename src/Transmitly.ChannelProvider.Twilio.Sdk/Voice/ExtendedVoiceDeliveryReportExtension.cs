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

using Transmitly.ChannelProvider.Twilio.Configuration.Voice;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Voice
{
	static class ExtendedVoiceDeliveryReportExtension
	{
		public static VoiceDeliveryReport ApplyExtendedProperties(this VoiceDeliveryReport voiceDeliveryReport, VoiceStatusReport report)
		{
			_ = new ExtendedVoiceDeliveryReportProperties(voiceDeliveryReport)
			{
				To = report.To,
				From = report.From,
				AccountSid = report.AccountSid,
				ApiVersion = report.ApiVersion,
				CallSid = report.CallSid,
				Duration = report.Duration,
				Timestamp = report.Timestamp,
				AnsweredBy = report.AnsweredBy,
				CallbackSource = report.CallbackSource,
				IdempotencyId = report.IdempotencyId,
				FromState = report.FromState,
				FromZip = report.FromZip,
				CalledState = report.CalledState,
				CallDuration = report.CallDuration,
				Called = report.Called,
				CalledCity = report.CalledCity,
				CalledCountry = report.CalledCountry,
				CalledZip = report.CalledZip,
				Caller = report.Caller,
				CallerCity = report.CallerCity,
				CallerCountry = report.CallerCountry,
				CallerState = report.CallerState,
				CallerZip = report.CallerZip,
				CallStatus = report.CallStatus,
				Direction = report.Direction,
				FromCity = report.FromCity,
				FromCountry = report.FromCountry,
				SequenceNumber = report.SequenceNumber,
				Signature = report.Signature,
				SipResponseCode = report.SipResponseCode,
				ToCity = report.ToCity,
				ToCountry = report.ToCountry,
				ToState = report.ToState,
				ToZip = report.ToZip
			};
			return voiceDeliveryReport;
		}
	}
}