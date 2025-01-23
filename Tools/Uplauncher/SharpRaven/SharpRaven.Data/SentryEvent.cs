using System;
using System.Collections.Generic;

namespace SharpRaven.Data
{
	public class SentryEvent
	{
		private readonly Exception exception;

		private IList<string> fingerprint;

		private SentryMessage message;

		private IDictionary<string, string> tags;

		public Exception Exception => exception;

		public object Extra { get; set; }

		public List<Breadcrumb> Breadcrumbs { get; set; }

		public IList<string> Fingerprint
		{
			get => fingerprint;
            internal set => fingerprint = value ?? new List<string>();
        }

		public ErrorLevel Level { get; set; }

		public SentryMessage Message
		{
			get => message ?? ((SentryMessage)((Exception != null) ? Exception.Message : null));
            set => message = value;
        }

		public IDictionary<string, string> Tags
		{
			get => tags;
            set => tags = value ?? new Dictionary<string, string>();
        }

		public SentryEvent(Exception exception)
			: this()
		{
			this.exception = exception;
			Level = ErrorLevel.Error;
		}

		public SentryEvent(SentryMessage message)
			: this()
		{
			Message = message;
		}

		public SentryEvent()
		{
			Tags = new Dictionary<string, string>();
			Fingerprint = new List<string>();
		}
	}
}
