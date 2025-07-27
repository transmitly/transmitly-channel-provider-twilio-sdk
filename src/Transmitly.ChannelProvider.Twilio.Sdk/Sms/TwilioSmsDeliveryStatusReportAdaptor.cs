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
using Transmitly.ChannelProvider.Twilio.Configuration.Sms;
using Transmitly.Delivery;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Sms
{
	public sealed class TwilioSmsDeliveryStatusReportAdaptor : IChannelProviderDeliveryReportRequestAdaptor
	{
		//https://www.twilio.com/docs/usage/webhooks/webhooks-security
		public Task<IReadOnlyCollection<DeliveryReport>?> AdaptAsync(IRequestAdaptorContext adaptorContext)
		{
			if (!ShouldAdapt(adaptorContext))
				return Task.FromResult<IReadOnlyCollection<DeliveryReport>?>(null);

			var smsReport = new SmsStatusReport(adaptorContext);

			var ret = new SmsDeliveryReport(
					DeliveryReport.Event.StatusChanged(),
					Id.Channel.Sms(),
					TwilioConstant.Id,
					adaptorContext.PipelineIntent,
					adaptorContext.PipelineId,
					smsReport.SmsSid,
					ConvertStatus(smsReport.SmsStatus),
					null,
					null,
					null
				).ApplyExtendedProperties(smsReport);

			return Task.FromResult<IReadOnlyCollection<DeliveryReport>?>(new List<DeliveryReport>() { ret }.AsReadOnly());
		}

		private static CommunicationsStatus ConvertStatus(SmsStatus? messageStatus)
		{
			return messageStatus switch
			{
				SmsStatus.Queued or SmsStatus.Sending =>
					CommunicationsStatus.Info(TwilioConstant.Id, Enum.GetName(typeof(SmsStatus), messageStatus) ?? "Pending", (int)messageStatus),

				SmsStatus.Sent or SmsStatus.Receiving or SmsStatus.Accepted or SmsStatus.Scheduled =>
					CommunicationsStatus.Success(TwilioConstant.Id, Enum.GetName(typeof(SmsStatus), messageStatus) ?? "Dispatched", (int)messageStatus),

				SmsStatus.Undelivered or SmsStatus.Failed =>
					CommunicationsStatus.ServerError(TwilioConstant.Id, Enum.GetName(typeof(SmsStatus), messageStatus) ?? "Failed", (int)messageStatus),

				SmsStatus.Received or SmsStatus.Delivered or SmsStatus.Read or SmsStatus.PartiallyDelivered =>
					CommunicationsStatus.Success(TwilioConstant.Id, Enum.GetName(typeof(SmsStatus), messageStatus) ?? "Delivered", (int)messageStatus),

				SmsStatus.Canceled =>
					CommunicationsStatus.ClientError(TwilioConstant.Id, Enum.GetName(typeof(SmsStatus), messageStatus) ?? "Canceled", (int)messageStatus),

				_ => CommunicationsStatus.ClientError(TwilioConstant.Id, "Unknown", CommunicationsStatus.ClientErrMax)
			};
		}

		private static bool ShouldAdapt(IRequestAdaptorContext adaptorContext)
		{
			return
				(adaptorContext.GetValue(DeliveryUtil.ChannelIdKey)?.Equals(Id.Channel.Sms(), StringComparison.InvariantCultureIgnoreCase) ?? false) &&
				(adaptorContext.GetValue(DeliveryUtil.ChannelProviderIdKey)?.StartsWith(TwilioConstant.Id, StringComparison.InvariantCultureIgnoreCase) ?? false);
		}
	}
}