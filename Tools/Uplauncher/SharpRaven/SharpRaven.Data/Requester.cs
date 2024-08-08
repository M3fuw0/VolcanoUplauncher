using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpRaven.Utilities;

namespace SharpRaven.Data
{
	public class Requester
	{
		private readonly RequestData data;

		private readonly JsonPacket packet;

		private readonly RavenClient ravenClient;

		private readonly HttpWebRequest webRequest;

		public IRavenClient Client => ravenClient;

		public RequestData Data => data;

		public JsonPacket Packet => packet;

		public HttpWebRequest WebRequest => webRequest;

		public async Task<string> RequestAsync()
		{
			using (Stream s = await webRequest.GetRequestStreamAsync())
			{
				if (Client.Compression)
				{
					await GzipUtil.WriteAsync(data.Scrubbed, s);
				}
				else
				{
					using (StreamWriter sw = new StreamWriter(s))
					{
						await sw.WriteAsync(data.Scrubbed);
					}
				}
			}
			HttpWebResponse httpWebResponse = (HttpWebResponse)(await webRequest.GetResponseAsync());
			using (HttpWebResponse wr = httpWebResponse)
			{
				using (Stream responseStream = wr.GetResponseStream())
				{
					if (responseStream == null)
					{
						return null;
					}
					using (StreamReader sr = new StreamReader(responseStream))
					{
						string content = await sr.ReadToEndAsync();
						dynamic response = JsonConvert.DeserializeObject<object>(content);
						return response.id;
					}
				}
			}
		}

		internal Requester(JsonPacket packet, RavenClient ravenClient)
		{
			if (packet == null)
			{
				throw new ArgumentNullException("packet");
			}
			if (ravenClient == null)
			{
				throw new ArgumentNullException("ravenClient");
			}
			this.ravenClient = ravenClient;
			this.packet = ravenClient.PreparePacket(packet);
			data = new RequestData(this);
			webRequest = (HttpWebRequest)System.Net.WebRequest.Create(ravenClient.CurrentDsn.SentryUri);
			webRequest.Timeout = (int)ravenClient.Timeout.TotalMilliseconds;
			webRequest.ReadWriteTimeout = (int)ravenClient.Timeout.TotalMilliseconds;
			webRequest.Method = "POST";
			webRequest.Accept = "application/json";
			webRequest.Headers.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(ravenClient.CurrentDsn));
			webRequest.UserAgent = PacketBuilder.UserAgent;
			if (ravenClient.Compression)
			{
				webRequest.Headers.Add(HttpRequestHeader.ContentEncoding, "gzip");
				webRequest.AutomaticDecompression = DecompressionMethods.Deflate;
				webRequest.ContentType = "application/octet-stream";
			}
			else
			{
				webRequest.ContentType = "application/json; charset=utf-8";
			}
		}

		public string Request()
		{
			using (Stream stream = webRequest.GetRequestStream())
			{
				if (ravenClient.Compression)
				{
					GzipUtil.Write(data.Scrubbed, stream);
				}
				else
				{
					using (StreamWriter streamWriter = new StreamWriter(stream))
					{
						streamWriter.Write(data.Scrubbed);
					}
				}
			}
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse())
			{
				using (Stream stream2 = httpWebResponse.GetResponseStream())
				{
					if (stream2 == null)
					{
						return null;
					}
					using (StreamReader streamReader = new StreamReader(stream2))
					{
						string text = streamReader.ReadToEnd();
						dynamic val = JsonConvert.DeserializeObject<object>(text);
						return val.id;
					}
				}
			}
		}
	}
}
