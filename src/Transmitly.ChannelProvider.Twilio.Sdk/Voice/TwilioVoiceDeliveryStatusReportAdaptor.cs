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
using System.Threading.Tasks;
using Transmitly.ChannelProvider.Twilio.Configuration;
using Transmitly.ChannelProvider.Twilio.Configuration.Voice;
using Transmitly.Delivery;


namespace Transmitly.ChannelProvider.Twilio.Sdk.Voice
{
	public sealed class TwilioVoiceDeliveryStatusReportAdaptor : IChannelProviderDeliveryReportRequestAdaptor
	{
		//todo: request validation
		//https://www.twilio.com/docs/usage/webhooks/webhooks-security
		public Task<IReadOnlyCollection<DeliveryReport>?> AdaptAsync(IRequestAdaptorContext adaptorContext)
		{
			if (!ShouldAdapt(adaptorContext))
				return Task.FromResult<IReadOnlyCollection<DeliveryReport>?>(null);

			var voiceReport = new VoiceStatusReport(adaptorContext);

			var ret = new VoiceDeliveryReport(
					DeliveryReport.Event.StatusChanged(),
					Id.Channel.Voice(),
					TwilioConstant.Id,
					adaptorContext.PipelineIntent,
					adaptorContext.PipelineId,
					voiceReport.CallSid,
					ConvertStatus(voiceReport.CallStatus),
					null,
					null,
					null
				).ApplyExtendedProperties(voiceReport);

			return Task.FromResult<IReadOnlyCollection<DeliveryReport>?>(new List<DeliveryReport>() { ret }.AsReadOnly());
		}

		private static CommunicationsStatus ConvertStatus(CallStatus? callStatus)
		{
			return callStatus switch
			{
				CallStatus.Unknown or CallStatus.Queued =>
					CommunicationsStatus.Success(TwilioConstant.Id, Enum.GetName(typeof(CallStatus), callStatus) ?? "Dispatched", (int)callStatus),

				CallStatus.Initiated or CallStatus.Ringing or CallStatus.InProgress =>
					CommunicationsStatus.Info(TwilioConstant.Id, Enum.GetName(typeof(CallStatus), callStatus) ?? "Pending", (int)callStatus),

				CallStatus.Completed =>
					CommunicationsStatus.Success(TwilioConstant.Id, Enum.GetName(typeof(CallStatus), callStatus) ?? "Delivered", (int)callStatus),

				CallStatus.Failed or CallStatus.Busy or CallStatus.NoAnswer =>
					CommunicationsStatus.ServerError(TwilioConstant.Id, Enum.GetName(typeof(CallStatus), callStatus) ?? "Failed", (int)callStatus),

				_ => CommunicationsStatus.ClientError(TwilioConstant.Id, "Unknown", CommunicationsStatus.ClientErrMax),
			};
		}

		private static bool ShouldAdapt(IRequestAdaptorContext adaptorContext)
		{
			return
				(adaptorContext.GetValue(DeliveryUtil.ChannelIdKey)?.Equals(Id.Channel.Voice(), StringComparison.InvariantCultureIgnoreCase) ?? false) &&
				(adaptorContext.GetValue(DeliveryUtil.ChannelProviderIdKey)?.StartsWith(TwilioConstant.Id, StringComparison.InvariantCultureIgnoreCase) ?? false);
		}
	}
}
