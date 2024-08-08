using System;
using System.Collections.Generic;

namespace SharpRaven.Data
{
	public class JsonPacketFactory : IJsonPacketFactory
	{
		[Obsolete("Use Create(string, SentryEvent) instead.")]
		public JsonPacket Create(string project, SentryMessage message, ErrorLevel level = ErrorLevel.Info, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null)
		{
			SentryEvent sentryEvent = new SentryEvent(message);
			sentryEvent.Level = level;
			sentryEvent.Extra = extra;
			sentryEvent.Tags = tags;
			sentryEvent.Fingerprint = fingerprint;
			SentryEvent @event = sentryEvent;
			return Create(project, @event);
		}

		[Obsolete("Use Create(string, SentryEvent) instead.")]
		public JsonPacket Create(string project, Exception exception, SentryMessage message = null, ErrorLevel level = ErrorLevel.Error, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null)
		{
			SentryEvent sentryEvent = new SentryEvent(exception);
			sentryEvent.Message = message;
			sentryEvent.Level = level;
			sentryEvent.Extra = extra;
			sentryEvent.Tags = tags;
			sentryEvent.Fingerprint = fingerprint;
			SentryEvent @event = sentryEvent;
			return Create(project, @event);
		}

		public JsonPacket Create(string project, SentryEvent @event)
		{
			JsonPacket jsonPacket = new JsonPacket(project, @event);
			jsonPacket.Breadcrumbs = @event.Breadcrumbs;
			JsonPacket jsonPacket2 = jsonPacket;
			return OnCreate(jsonPacket2);
		}

		protected virtual JsonPacket OnCreate(JsonPacket jsonPacket)
		{
			return jsonPacket;
		}
	}
}
