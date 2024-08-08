using System;
using SharpRaven.Utilities;

namespace SharpRaven
{
	public class Dsn
	{
		private readonly string path;

		private readonly int port;

		private readonly string privateKey;

		private readonly string projectID;

		private readonly string publicKey;

		private readonly Uri sentryUri;

		private readonly Uri uri;

		public string Path => path;

		public int Port => port;

		public string PrivateKey => privateKey;

		public string ProjectID => projectID;

		public string PublicKey => publicKey;

		public Uri SentryUri => sentryUri;

		public Uri Uri => uri;

		public Dsn(string dsn)
		{
			if (SystemUtil.IsNullOrWhiteSpace(dsn))
			{
				throw new ArgumentNullException("dsn");
			}
			try
			{
				uri = new Uri(dsn);
				privateKey = GetPrivateKey(uri);
				publicKey = GetPublicKey(uri);
				port = uri.Port;
				projectID = GetProjectID(uri);
				path = GetPath(uri);
				string uriString = $"{uri.Scheme}://{uri.DnsSafeHost}:{Port}{Path}/api/{ProjectID}/store/";
				sentryUri = new Uri(uriString);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Invalid DSN", "dsn", innerException);
			}
		}

		public override string ToString()
		{
			return uri.ToString();
		}

		private static string GetPath(Uri uri)
		{
			int length = uri.AbsolutePath.LastIndexOf("/", StringComparison.Ordinal);
			return uri.AbsolutePath.Substring(0, length);
		}

		private static string GetPrivateKey(Uri uri)
		{
			return uri.UserInfo.Split(':')[1];
		}

		private static string GetProjectID(Uri uri)
		{
			int num = uri.AbsoluteUri.LastIndexOf("/", StringComparison.Ordinal);
			return uri.AbsoluteUri.Substring(num + 1);
		}

		private static string GetPublicKey(Uri uri)
		{
			return uri.UserInfo.Split(':')[0];
		}
	}
}
