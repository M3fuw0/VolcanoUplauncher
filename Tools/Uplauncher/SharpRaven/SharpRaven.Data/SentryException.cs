using System;
using System.Text;
using Newtonsoft.Json;

namespace SharpRaven.Data
{
	public class SentryException
	{
		private readonly string message;

		[JsonProperty(PropertyName = "module")]
		public string Module { get; set; }

		[JsonProperty(PropertyName = "stacktrace")]
		public SentryStacktrace Stacktrace { get; set; }

		[JsonProperty(PropertyName = "type")]
		public string Type { get; set; }

		[JsonProperty(PropertyName = "value")]
		public string Value { get; set; }

		public SentryException(Exception exception)
		{
			if (exception != null)
			{
				message = exception.Message;
				Module = exception.Source;
				Type = exception.GetType().FullName;
				Value = exception.Message;
				Stacktrace = new SentryStacktrace(exception);
				if (Stacktrace.Frames == null || Stacktrace.Frames.Length == 0)
				{
					Stacktrace = null;
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (Type != null)
			{
				stringBuilder.Append(Type);
			}
			if (message != null)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(": ");
				}
				stringBuilder.Append(message);
				stringBuilder.AppendLine();
			}
			if (Stacktrace != null)
			{
				stringBuilder.Append(Stacktrace);
			}
			return stringBuilder.ToString().TrimEnd();
		}
	}
}
