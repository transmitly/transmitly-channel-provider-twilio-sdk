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

using Transmitly.ChannelProvider.Twilio.Configuration.Sms;
using Transmitly.ChannelProvider.Twilio.Configuration.Voice;
using Transmitly.ChannelProvider.Twilio.Sdk.Sms;
using Transmitly.ChannelProvider.Twilio.Sdk.Voice;
using Transmitly.Delivery;

namespace Transmitly.ChannelProvider.Twilio.Sdk
{
	/// <summary>
	/// Extend delivery properties for available channels.
	/// </summary>
	public sealed class DeliveryReportExtendedProperties : IDeliveryReportExtendedProperties
	{
		public DeliveryReportExtendedProperties()
		{

		}
		private DeliveryReportExtendedProperties(DeliveryReport deliveryReport)
		{
			Sms = new ExtendedSmsDeliveryReportProperties(deliveryReport);
			Voice = new ExtendedVoiceDeliveryReportProperties(deliveryReport);
		}

		/// <summary>
		/// Gets SMS extended properties for the delivery report.
		/// </summary>
		public IExtendedSmsDeliveryReportProperties Sms { get; }
		/// <summary>
		/// Gets Voice extended properties for the delivery report.
		/// </summary>
		public IExtendedVoiceDeliveryReportProperties Voice { get; }

		public IDeliveryReportExtendedProperties Adapt(DeliveryReport report)
		{
			return new DeliveryReportExtendedProperties(report);
		}
	}
}