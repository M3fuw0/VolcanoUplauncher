using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharpRaven.Utilities;

namespace SharpRaven.Data
{
	public class ExceptionData : Dictionary<string, object>
	{
		private readonly string exceptionType;

		[JsonProperty("type")]
		public string ExceptionType => exceptionType;

		public ExceptionData(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			exceptionType = exception.GetType().FullName;
			foreach (object key2 in exception.Data.Keys)
			{
				try
				{
					object value = exception.Data[key2];
					string key = (key2 as string) ?? key2.ToString();
					Add(key, value);
				}
				catch (Exception exception2)
				{
					SystemUtil.WriteError(exception2);
				}
			}
			if (exception.InnerException != null)
			{
				ExceptionData exceptionData = new ExceptionData(exception.InnerException);
				if (exceptionData.Count != 0)
				{
					exceptionData.AddTo(this);
				}
			}
		}

		private void AddTo(IDictionary<string, object> dictionary)
		{
			string key = ExceptionType + '.' + "Data";
			key = UniqueKey(dictionary, key);
			dictionary.Add(key, this);
		}

		private static string UniqueKey(IDictionary<string, object> dictionary, object key)
		{
			string text = (key as string) ?? key.ToString();
			if (!dictionary.ContainsKey(text))
			{
				return text;
			}
			for (int i = 0; i < 10000; i++)
			{
				string text2 = text + i;
				if (!dictionary.ContainsKey(text2))
				{
					return text2;
				}
			}
			throw new ArgumentException($"Unable to find a unique key for '{text}'.", "key");
		}
	}
}
