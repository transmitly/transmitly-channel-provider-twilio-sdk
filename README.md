# Transmitly.ChannelProvider.Twilio.Sdk

Twilio SDK-based SMS and voice dispatcher implementation for [Transmitly](https://github.com/transmitly/transmitly).

## Recommended Package

Most users should use the convenience package instead:

- [Transmitly.ChannelProvider.Twilio](https://github.com/transmitly/transmitly-channel-provider-twilio)

That package registers this SDK implementation for you.

## What This Package Provides

- Twilio-specific settings (`sms.Twilio()`, `voice.Twilio()`) and delivery-report values (`report.Twilio()`)


## Using This Package Directly (Advanced)

```csharp
using Transmitly;
using Transmitly.ChannelProvider.Twilio.Configuration;
using Transmitly.ChannelProvider.Twilio.Sdk;
using Transmitly.ChannelProvider.Twilio.Sdk.Sms;
using Transmitly.ChannelProvider.Twilio.Sdk.Voice;

var builder = new CommunicationsClientBuilder();

var options = new TwilioClientOptions
{
	AccountSid = "your-account-sid",
	AuthToken = "your-auth-token"
};

builder.ChannelProvider
	.Build(Id.ChannelProvider.Twilio(), options)
	.AddDispatcher<TwilioSmsChannelProviderDispatcher, ISms>(Id.Channel.Sms())
	.AddDispatcher<TwilioVoiceChannelProviderDispatcher, IVoice>(Id.Channel.Voice())
	.AddDeliveryReportRequestAdaptor<TwilioSmsDeliveryStatusReportAdaptor>()
	.AddDeliveryReportRequestAdaptor<TwilioVoiceDeliveryStatusReportAdaptor>()
	.Register();
```

## Related Packages

- [Transmitly](https://github.com/transmitly/transmitly)
- [Transmitly.ChannelProvider.Twilio](https://github.com/transmitly/transmitly-channel-provider-twilio)
- [Transmitly.ChannelProvider.Twilio.Configuration](https://github.com/transmitly/transmitly-channel-provider-twilio-configuration)

---
_Copyright (c) Code Impressions, LLC. This open-source project is sponsored and maintained by Code Impressions and is licensed under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._
