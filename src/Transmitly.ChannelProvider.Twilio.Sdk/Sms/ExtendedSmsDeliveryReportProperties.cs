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

using Transmitly.ChannelProvider.Twilio.Configuration;
using Transmitly.ChannelProvider.Twilio.Configuration.Sms;
using Transmitly.Delivery;
using Transmitly.Util;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Sms
{
	sealed class ExtendedSmsDeliveryReportProperties : IExtendedSmsDeliveryReportProperties
	{
		private readonly IExtendedProperties _extendedProperties;
		private const string ProviderKey = TwilioConstant.SmsPropertiesKey;
		internal ExtendedSmsDeliveryReportProperties(DeliveryReport deliveryReport)
		{
			_extendedProperties = Guard.AgainstNull(deliveryReport).ExtendedProperties;
		}

		internal ExtendedSmsDeliveryReportProperties(IExtendedProperties properties)
		{
			_extendedProperties = Guard.AgainstNull(properties);
		}

		internal void Apply(SmsStatusReport report)
		{
			To = report.To;
			From = report.From;
			ApiVersion = report.ApiVersion;
			AccountSid = report.AccountSid;
			IdempotencyId = report.IdempotencyId;
			Signature = report.Signature;
			HomeRegion = report.HomeRegion;
			MessageStatus = report.MessageStatus;
			SmsStatus = report.SmsStatus;
			SmsSid = report.SmsSid;
			MessageSid = report.MessageSid;
			ErrorCode = report.ErrorCode;
		}

		public string? From
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(From));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(From), value);
		}

		public string? To
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(To));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(To), value);
		}


		public string? ApiVersion
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(ApiVersion));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(ApiVersion), value);
		}

		/// <summary>
		/// Your Twilio account ID. It is 34 characters long, and always starts with the letters AC.
		/// </summary>
		public string? AccountSid
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(AccountSid));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(AccountSid), value);
		}

		public string? IdempotencyId
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(IdempotencyId));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(IdempotencyId), value);
		}
		public string? Signature
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Signature));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Signature), value);
		}
		public string? HomeRegion
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(HomeRegion));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(HomeRegion), value);
		}

		public SmsStatus? MessageStatus
		{
			get => _extendedProperties.GetValue<SmsStatus?>(ProviderKey, nameof(MessageStatus));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(MessageStatus), value);
		}
		public SmsStatus? SmsStatus
		{
			get => _extendedProperties.GetValue<SmsStatus?>(ProviderKey, nameof(SmsStatus));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(SmsStatus), value);
		}
		public string? SmsSid
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(SmsSid));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(SmsSid), value);
		}
		public string? MessageSid
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(MessageSid));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(MessageSid), value);
		}

		public string? ErrorCode
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(ErrorCode));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(ErrorCode), value);
		}
	}
}