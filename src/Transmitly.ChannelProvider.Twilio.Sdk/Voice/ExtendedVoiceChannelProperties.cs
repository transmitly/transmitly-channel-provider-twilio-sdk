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
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Transmitly.Channel.Configuration;
using Transmitly.ChannelProvider.Twilio.Configuration;
using Transmitly.ChannelProvider.Twilio.Configuration.Voice;
using Transmitly.Util;

namespace Transmitly.ChannelProvider.Twilio.Sdk.Voice
{
	/// <summary>
	/// Twilio specific properties available through channel provider extensions
	/// </summary>
	public sealed class ExtendedVoiceChannelProperties : IExtendedVoiceChannelProperties
	{
		private readonly IExtendedProperties _extendedProperties;
		private const string ProviderKey = TwilioConstant.VoicePropertiesKey;

		public ExtendedVoiceChannelProperties()
		{

		}

		private ExtendedVoiceChannelProperties(IChannel<IVoice> voiceChannel)
		{
			Guard.AgainstNull(voiceChannel);
			_extendedProperties = Guard.AgainstNull(voiceChannel.ExtendedProperties);

		}

		internal ExtendedVoiceChannelProperties(IExtendedProperties properties)
		{
			_extendedProperties = Guard.AgainstNull(properties);
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
		/// Enable machine detection or end of greeting detection.
		/// </summary>
		public MachineDetection? MachineDetection
		{
			get => _extendedProperties.GetValue<MachineDetection>(ProviderKey, nameof(MachineDetection));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(MachineDetection), value);
		}

		/// <summary>
		/// Number of seconds to wait for an answer
		/// </summary>
		public int? Timeout
		{
			get => _extendedProperties.GetValue<int?>(ProviderKey, nameof(Timeout));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Timeout), value);
		}

		/// <summary>
		/// A resolver that will return the absolute URL that returns TwiML for this call.
		/// This will override any value set in the <see cref="Url"/> property.
		/// </summary>
		public Func<IDispatchCommunicationContext, Task<string?>>? UrlResolver
		{
			get => _extendedProperties.GetValue<Func<IDispatchCommunicationContext, Task<string?>>>(ProviderKey, nameof(UrlResolver));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(UrlResolver), value);
		}

		/// <summary>
		/// The absolute URL that returns TwiML for this call. The communication's ResourceId will be added to the querystring.
		/// <para>Example: https://yourUrl.com/path?<strong>resourceId=1234abc</strong></para>
		/// <para>If you require dynamic resolution see: <see cref="UrlResolver"/>. Setting the <see cref="UrlResolver"/> will override this value.</para>
		/// </summary>
		public string? Url
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Url));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Url), value);
		}

		/// <summary>
		/// HTTP method to use to fetch TwiML.
		/// </summary>
		public HttpMethod? UrlMethod
		{
			get => _extendedProperties.GetValue<HttpMethod?>(ProviderKey, nameof(UrlMethod));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(UrlMethod), value);
		}

		/// <summary>
		/// Called upon dispatch of message for storage prior to Twilio retrieval. 
		/// <para><seealso cref="UrlResolver"/> or <seealso cref="Url"/></para>
		/// </summary>
		public Func<string, IVoice, IDispatchCommunicationContext, Task>? OnStoreMessageForRetrievalAsync
		{
			get => _extendedProperties.GetValue<Func<string, IVoice, IDispatchCommunicationContext, Task>?>(ProviderKey, nameof(OnStoreMessageForRetrievalAsync));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(OnStoreMessageForRetrievalAsync), value);
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return [];
		}

		string? ICustomTypeDescriptor.GetClassName()
		{
			return nameof(ExtendedVoiceChannelProperties);
		}

		string? ICustomTypeDescriptor.GetComponentName()
		{
			return nameof(ExtendedVoiceChannelProperties);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(typeof(ExtendedVoiceChannelProperties));
		}

		EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object? ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return new EventDescriptorCollection([]);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[]? attributes)
		{
			return new EventDescriptorCollection([]);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return GetProperties();
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[]? attributes)
		{
			return GetProperties();
		}

		private static PropertyDescriptorCollection GetProperties()
		{
			var props = typeof(ExtendedVoiceChannelProperties).GetProperties().Where(x => x.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), false).Length != 0);
			var descriptors = props.Select(s => (PropertyDescriptor)new ExtendedVoiceChannelPropertiesPropertyDescriptor(s.Name, [])).ToArray();
			return new PropertyDescriptorCollection(descriptors);
		}

		object? ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor? pd)
		{
			return this;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public IExtendedVoiceChannelProperties Adapt(IChannel<IVoice> sms)
		{
			return new ExtendedVoiceChannelProperties(sms);
		}
	}

	/// <summary>
	/// Property descriptor for <see cref="ExtendedVoiceChannelProperties"/>.
	/// </summary>
	/// <param name="name">Property name.</param>
	/// <param name="attrs">Attributes on property.</param>
	public class ExtendedVoiceChannelPropertiesPropertyDescriptor(string name, Attribute[] attrs) : PropertyDescriptor(name, attrs)
	{
		public override Type ComponentType
		{
			get { return typeof(ExtendedVoiceChannelProperties); }
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get { return typeof(string); }
		}

		public override bool CanResetValue(object component)
		{
			return GetValue(component).Equals("");
		}

		public override void ResetValue(object component)
		{
			SetValue(component, "");
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			return component;
		}

		public override void SetValue(object component, object value)
		{

		}
	}
}