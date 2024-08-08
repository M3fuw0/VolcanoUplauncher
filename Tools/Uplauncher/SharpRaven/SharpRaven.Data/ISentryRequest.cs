using System.Collections.Generic;

namespace SharpRaven.Data
{
	public interface ISentryRequest
	{
		IDictionary<string, string> Cookies { get; set; }

		object Data { get; set; }

		IDictionary<string, string> Environment { get; set; }

		IDictionary<string, string> Headers { get; set; }

		string Method { get; set; }

		string QueryString { get; set; }

		string Url { get; set; }
	}
}
