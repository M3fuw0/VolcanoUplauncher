using System.Linq;
using Newtonsoft.Json;

namespace SharpRaven.Data
{
	public class SentryMessage
	{
		private readonly string message;

		private readonly object[] parameters;

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Message => message;

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public object[] Parameters => parameters;

		public SentryMessage(string format, params object[] parameters)
			: this(format)
		{
			this.parameters = parameters;
		}

		public SentryMessage(string message)
		{
			this.message = message;
		}

		public override string ToString()
		{
			if (message != null && parameters != null && parameters.Any())
			{
				try
				{
					return string.Format(message, parameters);
				}
				catch
				{
					return message;
				}
			}
			return message ?? string.Empty;
		}

		public static implicit operator SentryMessage(string message)
		{
			if (message == null)
			{
				return null;
			}
			return new SentryMessage(message);
		}

		public static implicit operator string(SentryMessage message)
		{
			return message?.ToString();
		}
	}
}
