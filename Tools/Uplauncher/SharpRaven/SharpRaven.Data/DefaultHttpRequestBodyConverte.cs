using System;
using System.IO;
using System.Text;

namespace SharpRaven.Data
{
	public class DefaultHttpRequestBodyConverter : IHttpRequestBodyConverter
	{
		public bool Matches(string contentType)
		{
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
				using (MemoryStream memoryStream = new MemoryStream())
				{
					httpContext.Request.InputStream.Seek(0, SeekOrigin.Begin);
					httpContext.Request.InputStream.CopyTo(memoryStream);
					converted = Encoding.UTF8.GetString(memoryStream.ToArray());
					return true;
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			return false;
		}
	}
}
