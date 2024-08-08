using System;
using System.Linq;

namespace SharpRaven.Data
{
	public class MultiPartFormHttpRequestBodyConverter : FormHttpRequestBodyConverter
	{
		public override bool Matches(string contentType)
		{
			if (string.IsNullOrEmpty(contentType))
			{
				return false;
			}
			string text = contentType.Split(';').First();
			return text.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase);
		}
	}
}
