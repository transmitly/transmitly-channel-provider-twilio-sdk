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
using Transmitly.Delivery;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Sms
{
	public sealed record SmsDeliveryReport : DeliveryReport, ISmsDeliveryReport
	{
		private readonly ExtendedSmsDeliveryReportProperties _extended;

		public SmsDeliveryReport(DeliveryReport original) : base(original)
		{
			_extended = new ExtendedSmsDeliveryReportProperties(this);
		}

		public SmsDeliveryReport(string EventName, string? ChannelId, string? ChannelProviderId, string? PipelineIntent, string? PipelineId,
				string? ResourceId, CommunicationsStatus Status, object? ChannelCommunication, IContentModel? ContentModel, Exception? Exception)
			: base(EventName, ChannelId, ChannelProviderId, PipelineIntent, PipelineId, ResourceId, Status, ChannelCommunication, ContentModel, Exception)
		{
			_extended = new ExtendedSmsDeliveryReportProperties(this);
		}

		public string? To => _extended.To;

		public string? From => _extended.From;

		public int? Count => null;
	}
}