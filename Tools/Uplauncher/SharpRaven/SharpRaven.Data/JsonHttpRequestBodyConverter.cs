using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SharpRaven.Data
{
	public class JsonHttpRequestBodyConverter : IHttpRequestBodyConverter
	{
		public bool Matches(string contentType)
		{
			if (string.IsNullOrEmpty(contentType))
			{
				return false;
			}
			string text = contentType.Split(';').First();
			if (!text.Equals("application/json", StringComparison.OrdinalIgnoreCase) && !text.StartsWith("application/json-", StringComparison.OrdinalIgnoreCase) && !text.Equals("text/json", StringComparison.OrdinalIgnoreCase))
			{
				return text.EndsWith("+json", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}

		public bool TryConvert(dynamic httpContext, out object converted)
		{
			converted = null;
			if (httpContext == null)
			{
				return false;
			}
			try
			{
				string text = null;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					httpContext.Request.InputStream.Seek(0, SeekOrigin.Begin);
					httpContext.Request.InputStream.CopyTo(memoryStream);
					text = Encoding.UTF8.GetString(memoryStream.ToArray());
				}
				converted = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
				return true;
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			return false;
		}
	}
}
