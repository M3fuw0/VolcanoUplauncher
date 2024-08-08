using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpRaven.Data;
using SharpRaven.Logging;

namespace SharpRaven
{
	public interface IRavenClient
	{
		Func<Requester, Requester> BeforeSend { get; set; }

		bool Compression { get; set; }

		Dsn CurrentDsn { get; }

		string Environment { get; set; }

		bool IgnoreBreadcrumbs { get; set; }

		string Logger { get; set; }

		IScrubber LogScrubber { get; set; }

		string Release { get; set; }

		IDictionary<string, string> Tags { get; }

		TimeSpan Timeout { get; set; }

		void AddTrail(Breadcrumb breadcrumb);

		string Capture(SentryEvent @event);

		[Obsolete("Use Capture(SentryEvent) instead.")]
		string CaptureException(Exception exception, SentryMessage message = null, ErrorLevel level = ErrorLevel.Error, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null);

		[Obsolete("Use Capture(SentryEvent) instead")]
		string CaptureMessage(SentryMessage message, ErrorLevel level = ErrorLevel.Info, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null);

		void RestartTrails();

		Task<string> CaptureAsync(SentryEvent @event);

		[Obsolete("Use CaptureAsync(SentryEvent) instead.")]
		Task<string> CaptureExceptionAsync(Exception exception, SentryMessage message = null, ErrorLevel level = ErrorLevel.Error, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null);

		[Obsolete("Use CaptureAsync(SentryEvent) instead.")]
		Task<string> CaptureMessageAsync(SentryMessage message, ErrorLevel level = ErrorLevel.Info, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null);

		[Obsolete("Use CaptureException() instead.", true)]
		string CaptureEvent(Exception e);

		[Obsolete("Use CaptureException() instead.", true)]
		string CaptureEvent(Exception e, Dictionary<string, string> tags);
	}
}
