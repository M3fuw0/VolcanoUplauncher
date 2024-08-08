using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using SharpRaven.Utilities;

namespace SharpRaven.Data
{
	public class SentryRequestFactory : ISentryRequestFactory
	{
		private static bool checkedForHttpContextProperty;

		internal static dynamic CurrentHttpContextProperty { get; set; }

		[JsonIgnore]
		internal static bool HasCurrentHttpContextProperty => CurrentHttpContextProperty != null;

		[JsonIgnore]
		internal static bool HasHttpContext => HttpContext != null;

		internal static dynamic HttpContext
		{
			get
			{
				TryGetHttpContextPropertyFromAppDomain();
				if (!HasCurrentHttpContextProperty)
				{
					return null;
				}
				try
				{
					return CurrentHttpContextProperty.GetValue(null, null);
				}
				catch (Exception exception)
				{
					SystemUtil.WriteError(exception);
					return null;
				}
			}
		}

		public ISentryRequest Create()
		{
			if (!HasHttpContext || HttpContext.Request == null)
			{
				return OnCreate(null);
			}
			SentryRequest sentryRequest = new SentryRequest();
			sentryRequest.Url = HttpContext.Request.Url.ToString();
			sentryRequest.Method = HttpContext.Request.HttpMethod;
			sentryRequest.Environment = Convert((dynamic x) => x.Request.ServerVariables);
			sentryRequest.Headers = Convert((dynamic x) => x.Request.Headers);
			sentryRequest.Cookies = Convert((dynamic x) => x.Request.Cookies);
			sentryRequest.Data = BodyConvert();
			sentryRequest.QueryString = HttpContext.Request.QueryString.ToString();
			SentryRequest request = sentryRequest;
			return OnCreate(request);
		}

		public virtual SentryRequest OnCreate(SentryRequest request)
		{
			return request;
		}

		private static object BodyConvert()
		{
			if (!HasHttpContext)
			{
				return null;
			}
			try
			{
				return HttpRequestBodyConverter.Convert(HttpContext);
			}
			catch (Exception exception)
			{
				SystemUtil.WriteError(exception);
			}
			return null;
		}

		private static IDictionary<string, string> Convert(Func<dynamic, NameObjectCollectionBase> collectionGetter)
		{
			if (!HasHttpContext)
			{
				return null;
			}
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			try
			{
				dynamic val = ((Func<object, NameObjectCollectionBase>)collectionGetter).Invoke(HttpContext);
				dynamic val2 = Enumerable.ToArray(val.AllKeys);
				foreach (object item in val2)
				{
					if (item == null)
					{
						continue;
					}
					string text = (item as string) ?? item.ToString();
					if (text.StartsWith("ALL_") || text.StartsWith("HTTP_"))
					{
						continue;
					}
					dynamic val3 = val[text];
					if (val3 is string value)
					{
						dictionary.Add(text, value);
						continue;
					}
					try
					{
						dictionary.Add(text, val3.Value);
					}
					catch (Exception ex)
					{
						dictionary.Add(text, ex.ToString());
					}
				}
				return dictionary;
			}
			catch (Exception exception)
			{
				SystemUtil.WriteError(exception);
				return dictionary;
			}
		}

		private static void TryGetHttpContextPropertyFromAppDomain()
		{
			if (checkedForHttpContextProperty)
			{
				return;
			}
			checkedForHttpContextProperty = true;
			try
			{
				Assembly assembly2 = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly assembly) => assembly.FullName.StartsWith("System.Web,"));
				if (assembly2 == null)
				{
					return;
				}
				Type type2 = assembly2.GetExportedTypes().FirstOrDefault((Type type) => type.Name == "HttpContext");
				if (!(type2 == null))
				{
					PropertyInfo property = type2.GetProperty("Current", BindingFlags.Static | BindingFlags.Public);
					if (!(property == null))
					{
						CurrentHttpContextProperty = property;
					}
				}
			}
			catch (Exception exception)
			{
				SystemUtil.WriteError(exception);
			}
		}
	}
}
