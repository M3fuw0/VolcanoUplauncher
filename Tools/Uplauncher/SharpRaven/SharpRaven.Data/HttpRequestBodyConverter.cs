using System.Collections.Generic;

namespace SharpRaven.Data
{
	public static class HttpRequestBodyConverter
	{
		public static object Convert(dynamic httpContext)
		{
			Dictionary<string, IHttpRequestBodyConverter> dictionary = new Dictionary<string, IHttpRequestBodyConverter>();
			dictionary.Add("FormMediaType", new FormHttpRequestBodyConverter());
			dictionary.Add("MultiPartFormMediaType", new MultiPartFormHttpRequestBodyConverter());
			dictionary.Add("JsonMediaType", new JsonHttpRequestBodyConverter());
			dictionary.Add("DefaultMediaType", new DefaultHttpRequestBodyConverter());
			Dictionary<string, IHttpRequestBodyConverter> dictionary2 = dictionary;
			foreach (KeyValuePair<string, IHttpRequestBodyConverter> item in dictionary2)
			{
				IHttpRequestBodyConverter value = item.Value;
				if (!((!value.Matches(httpContext.Request.ContentType)) ? true : false) && (value.TryConvert(httpContext, out object result) ? true : false))
				{
					return result;
				}
			}
			return null;
		}
	}
}
