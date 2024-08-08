using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SharpRaven.Data;
using SharpRaven.Logging;
using SharpRaven.Utilities;

namespace SharpRaven
{
	public class RavenClient : IRavenClient
	{
		private readonly CircularBuffer<Breadcrumb> breadcrumbs;

		private readonly Dsn currentDsn;

		private readonly IDictionary<string, string> defaultTags;

		private readonly IJsonPacketFactory jsonPacketFactory;

		private readonly ISentryRequestFactory sentryRequestFactory;

		private readonly ISentryUserFactory sentryUserFactory;

		public Func<Requester, Requester> BeforeSend { get; set; }

		public Action<Exception> ErrorOnCapture { get; set; }

		public bool Compression { get; set; }

		public Dsn CurrentDsn => currentDsn;

		public IScrubber LogScrubber { get; set; }

		public string Logger { get; set; }

		public string Release { get; set; }

		public string Environment { get; set; }

		public IDictionary<string, string> Tags => defaultTags;

		public TimeSpan Timeout { get; set; }

		public bool IgnoreBreadcrumbs { get; set; }

		public async Task<string> CaptureAsync(SentryEvent @event)
		{
			@event.Tags = MergeTags(@event.Tags);
			if (!breadcrumbs.IsEmpty())
			{
				@event.Breadcrumbs = breadcrumbs.ToList();
			}
			JsonPacket packet = jsonPacketFactory.Create(CurrentDsn.ProjectID, @event);
			string eventId = await SendAsync(packet);
			RestartTrails();
			return eventId;
		}

		[Obsolete("Use CaptureAsync(SentryEvent) instead.")]
		public async Task<string> CaptureExceptionAsync(Exception exception, SentryMessage message = null, ErrorLevel level = ErrorLevel.Error, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null)
		{
			SentryEvent @event = new SentryEvent(exception)
			{
				Message = message,
				Level = level,
				Extra = extra,
				Tags = tags,
				Fingerprint = fingerprint
			};
			return await CaptureAsync(@event);
		}

		[Obsolete("Use CaptureAsync(SentryEvent) instead.")]
		public async Task<string> CaptureMessageAsync(SentryMessage message, ErrorLevel level = ErrorLevel.Info, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null)
		{
			SentryEvent @event = new SentryEvent(message)
			{
				Level = level,
				Extra = extra,
				Tags = tags,
				Fingerprint = fingerprint
			};
			return await CaptureAsync(@event);
		}

		protected virtual async Task<string> SendAsync(JsonPacket packet)
		{
			Requester requester = null;
			try
			{
				requester = new Requester(packet, this);
				if (BeforeSend != null)
				{
					requester = BeforeSend(requester);
				}
				return await requester.RequestAsync();
			}
			catch (Exception exception)
			{
				return HandleException(exception, requester);
			}
		}

		public RavenClient(IJsonPacketFactory jsonPacketFactory = null)
			: this(new Dsn(Configuration.Settings.Dsn.Value), jsonPacketFactory)
		{
		}

		public RavenClient(string dsn, IJsonPacketFactory jsonPacketFactory = null, ISentryRequestFactory sentryRequestFactory = null, ISentryUserFactory sentryUserFactory = null)
			: this(new Dsn(dsn), jsonPacketFactory, sentryRequestFactory, sentryUserFactory)
		{
		}

		public RavenClient(Dsn dsn, IJsonPacketFactory jsonPacketFactory = null, ISentryRequestFactory sentryRequestFactory = null, ISentryUserFactory sentryUserFactory = null)
		{
			if (dsn == null)
			{
				throw new ArgumentNullException("dsn");
			}
			currentDsn = dsn;
			this.jsonPacketFactory = jsonPacketFactory ?? new JsonPacketFactory();
			this.sentryRequestFactory = sentryRequestFactory ?? new SentryRequestFactory();
			this.sentryUserFactory = sentryUserFactory ?? new SentryUserFactory();
			Logger = "root";
			Timeout = TimeSpan.FromSeconds(5.0);
			defaultTags = new Dictionary<string, string>();
			breadcrumbs = new CircularBuffer<Breadcrumb>();
		}

		public void AddTrail(Breadcrumb breadcrumb)
		{
			if (!IgnoreBreadcrumbs && breadcrumb != null)
			{
				breadcrumbs.Add(breadcrumb);
			}
		}

		public void RestartTrails()
		{
			breadcrumbs.Clear();
		}

		public string Capture(SentryEvent @event)
		{
			if (@event == null)
			{
				throw new ArgumentNullException("event");
			}
			@event.Tags = MergeTags(@event.Tags);
			if (!breadcrumbs.IsEmpty())
			{
				@event.Breadcrumbs = breadcrumbs.ToList();
			}
			JsonPacket packet = jsonPacketFactory.Create(CurrentDsn.ProjectID, @event);
			string result = Send(packet);
			RestartTrails();
			return result;
		}

		[Obsolete("Use CaptureException() instead.", true)]
		public string CaptureEvent(Exception e)
		{
			return CaptureException(e);
		}

		[Obsolete("Use CaptureException() instead.", true)]
		public string CaptureEvent(Exception e, Dictionary<string, string> tags)
		{
			return CaptureException(e, null, ErrorLevel.Error, tags);
		}

		[Obsolete("Use Capture(SentryEvent) instead")]
		public string CaptureException(Exception exception, SentryMessage message = null, ErrorLevel level = ErrorLevel.Error, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null)
		{
			SentryEvent sentryEvent = new SentryEvent(exception);
			sentryEvent.Message = message;
			sentryEvent.Level = level;
			sentryEvent.Extra = extra;
			sentryEvent.Tags = MergeTags(tags);
			sentryEvent.Fingerprint = fingerprint;
			SentryEvent @event = sentryEvent;
			return Capture(@event);
		}

		[Obsolete("Use Capture(SentryEvent) instead")]
		public string CaptureMessage(SentryMessage message, ErrorLevel level = ErrorLevel.Info, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null)
		{
			SentryEvent sentryEvent = new SentryEvent(message);
			sentryEvent.Level = level;
			sentryEvent.Extra = extra;
			sentryEvent.Tags = MergeTags(tags);
			sentryEvent.Fingerprint = fingerprint;
			SentryEvent @event = sentryEvent;
			return Capture(@event);
		}

		protected internal virtual JsonPacket PreparePacket(JsonPacket packet)
		{
			packet.Logger = ((SystemUtil.IsNullOrWhiteSpace(packet.Logger) || (packet.Logger == "root" && !SystemUtil.IsNullOrWhiteSpace(Logger))) ? Logger : packet.Logger);
			packet.User = packet.User ?? sentryUserFactory.Create();
			try
			{
				packet.Request = packet.Request ?? sentryRequestFactory.Create();
			}
			catch (Exception exception)
			{
				HandleException(exception, null);
				packet.Request = null;
			}
			packet.Release = (SystemUtil.IsNullOrWhiteSpace(packet.Release) ? Release : packet.Release);
			packet.Environment = (SystemUtil.IsNullOrWhiteSpace(packet.Environment) ? Environment : packet.Environment);
			return packet;
		}

		protected virtual string Send(JsonPacket packet)
		{
			Requester requester = null;
			try
			{
				requester = new Requester(packet, this);
				if (BeforeSend != null)
				{
					requester = BeforeSend(requester);
				}
				return requester.Request();
			}
			catch (Exception exception)
			{
				return HandleException(exception, requester);
			}
		}

		private string HandleException(Exception exception, Requester requester)
		{
			string text = null;
			try
			{
				if (ErrorOnCapture != null)
				{
					ErrorOnCapture(exception);
					return null;
				}
				if (exception != null)
				{
					SystemUtil.WriteError(exception);
				}
				if (requester != null)
				{
					if (requester.Data != null)
					{
						SystemUtil.WriteError("Request body (raw):", requester.Data.Raw);
						SystemUtil.WriteError("Request body (scrubbed):", requester.Data.Scrubbed);
					}
					if (requester.WebRequest != null && requester.WebRequest.Headers != null && requester.WebRequest.Headers.Count > 0)
					{
						SystemUtil.WriteError("Request headers:", requester.WebRequest.Headers.ToString());
					}
				}
				if (!(exception is WebException ex) || ex.Response == null)
				{
					return null;
				}
				WebResponse response = ex.Response;
				text = response.Headers["X-Sentry-ID"];
				if (SystemUtil.IsNullOrWhiteSpace(text))
				{
					text = null;
				}
				string multilineData;
				using (Stream stream = response.GetResponseStream())
				{
					if (stream == null)
					{
						return text;
					}
					using (StreamReader streamReader = new StreamReader(stream))
					{
						multilineData = streamReader.ReadToEnd();
					}
				}
				SystemUtil.WriteError("Response headers:", response.Headers.ToString());
				SystemUtil.WriteError("Response body:", multilineData);
				return text;
			}
			catch (Exception ex2)
			{
				SystemUtil.WriteError(ex2.ToString());
				return text;
			}
		}

		private IDictionary<string, string> MergeTags(IDictionary<string, string> tags = null)
		{
			if (tags == null)
			{
				return Tags;
			}
			return Tags.Where((KeyValuePair<string, string> kv) => !tags.Keys.Contains(kv.Key)).Concat(tags).ToDictionary((KeyValuePair<string, string> kv) => kv.Key, (KeyValuePair<string, string> kv) => kv.Value);
		}
	}
}
