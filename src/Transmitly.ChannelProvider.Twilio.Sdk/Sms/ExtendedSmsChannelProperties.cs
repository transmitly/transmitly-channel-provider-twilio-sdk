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
using System.Threading.Tasks;
using Transmitly.Channel.Configuration;
using Transmitly.ChannelProvider.Twilio.Configuration;
using Transmitly.ChannelProvider.Twilio.Configuration.Sms;
using Transmitly.Util;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Sms
{
	/// <summary>
	/// Twilio specific SMS channel properties 
	/// </summary>
	public sealed class ExtendedSmsChannelProperties : IExtendedSmsChannelProperties
	{
		private readonly IExtendedProperties _extendedProperties;
		private const string ProviderKey = TwilioConstant.SmsPropertiesKey;

		public ExtendedSmsChannelProperties()
		{

		}
		private ExtendedSmsChannelProperties(IChannel<ISms> smsChannel)
		{
			Guard.AgainstNull(smsChannel);
			_extendedProperties = Guard.AgainstNull(smsChannel.ExtendedProperties);

		}

		internal ExtendedSmsChannelProperties(IExtendedProperties properties)
		{
			_extendedProperties = properties;
		}

		/// <summary>
		/// The URL we should call to send status information to your application.
		/// </summary>
		public string? StatusCallbackUrl
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(StatusCallbackUrl));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(StatusCallbackUrl), value);
		}

		/// <summary>
		/// A resolver that will return the URL we should call to send status information to your application.
		/// </summary>
		public Func<IDispatchCommunicationContext, Task<string?>>? StatusCallbackUrlResolver
		{
			get => _extendedProperties.GetValue<Func<IDispatchCommunicationContext, Task<string?>>?>(ProviderKey, nameof(StatusCallbackUrlResolver));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(StatusCallbackUrlResolver), value);
		}

		/// <summary>
		/// HTTP method to use to send status information to your application.
		/// </summary>
		public string? StatusCallbackMethod
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(StatusCallbackMethod));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(StatusCallbackMethod), value);
		}

		/// <summary>
		/// The SID of the Messaging Service you want to associate with the message.
		/// </summary>
		public string? MessagingServiceSid
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(MessagingServiceSid));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(MessagingServiceSid), value);
		}

		public IExtendedSmsChannelProperties Adapt(IChannel<ISms> sms)
		{
			return new ExtendedSmsChannelProperties(sms);
		}
	}
}