using System;
using Newtonsoft.Json;

namespace SharpRaven.Data
{
	public class RequestData
	{
		private readonly Requester requester;

		private string formatted;

		private string raw;

		private string scrubbed;

		public string Raw
		{
			get
			{
				string obj = raw ?? requester.Packet.ToString((Formatting)0);
				string result = obj;
				raw = obj;
				return result;
			}
		}

		public string Scrubbed
		{
			get
			{
				if (requester.Client == null || requester.Client.LogScrubber == null)
				{
					return Raw;
				}
				return scrubbed = scrubbed ?? requester.Client.LogScrubber.Scrub(Raw);
			}
		}

		internal RequestData(Requester requester)
		{
			if (requester == null)
			{
				throw new ArgumentNullException("requester");
			}
			if (requester.Packet == null)
			{
				throw new ArgumentException("Requester.Packet was null", "requester");
			}
			this.requester = requester;
		}

		public override string ToString()
		{
			string obj = formatted ?? requester.Packet.ToString((Formatting)1);
			string result = obj;
			formatted = obj;
			return result;
		}
	}
}
