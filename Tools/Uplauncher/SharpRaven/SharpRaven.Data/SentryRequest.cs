using System.Collections.Generic;
using Newtonsoft.Json;

namespace SharpRaven.Data
{
	public class SentryRequest : ISentryRequest
	{
		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public IDictionary<string, string> Cookies { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public object Data { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public IDictionary<string, string> Environment { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public IDictionary<string, string> Headers { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Method { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string QueryString { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Url { get; set; }

		public static ISentryRequest GetRequest(ISentryRequestFactory factory)
		{
			return factory?.Create();
		}
	}
}
