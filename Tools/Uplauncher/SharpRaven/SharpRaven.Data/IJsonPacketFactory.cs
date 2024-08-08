using System;
using System.Collections.Generic;

namespace SharpRaven.Data
{
	public interface IJsonPacketFactory
	{
		[Obsolete("Use Create(string, SentryEvent) instead.")]
		JsonPacket Create(string project, SentryMessage message, ErrorLevel level = ErrorLevel.Info, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null);

		[Obsolete("Use Create(string, SentryEvent) instead.")]
		JsonPacket Create(string project, Exception exception, SentryMessage message = null, ErrorLevel level = ErrorLevel.Error, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null);

		JsonPacket Create(string project, SentryEvent @event);
	}
}
