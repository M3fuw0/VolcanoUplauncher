using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpRaven.Data
{
	public class FormHttpRequestBodyConverter : IHttpRequestBodyConverter
	{
		public virtual bool Matches(string contentType)
		{
			if (string.IsNullOrEmpty(contentType))
			{
				return false;
			}
			string text = contentType.Split(';').First();
			return text.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase);
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
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dynamic val = httpContext.Request.Form;
				dynamic val2 = Enumerable.ToArray(val.AllKeys);
				foreach (object item in val2)
				{
					if (item != null)
					{
						string text = (item as string) ?? item.ToString();
						object obj = val[text];
						string value = obj as string;
						dictionary.Add(text, value);
					}
				}
				converted = dictionary;
				return true;
			}
			catch (Exception value2)
			{
				Console.WriteLine(value2);
			}
			return false;
		}
	}
}
