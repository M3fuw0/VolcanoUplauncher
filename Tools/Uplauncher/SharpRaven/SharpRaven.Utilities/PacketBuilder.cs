using System;
using System.Reflection;

namespace SharpRaven.Utilities
{
	public static class PacketBuilder
	{
		private const int SentryVersion = 7;

		private static readonly string userAgent;

		public static string UserAgent => userAgent;

		static PacketBuilder()
		{
			AssemblyName name = typeof(PacketBuilder).Assembly.GetName();
			string name2 = name.Name;
			Version version = name.Version;
			userAgent = $"{name2}/{version}";
		}

		public static string CreateAuthenticationHeader(Dsn dsn)
		{
			return $"Sentry sentry_version={7}, sentry_client={UserAgent}, sentry_timestamp={(long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds}, sentry_key={dsn.PublicKey}, sentry_secret={dsn.PrivateKey}";
		}
	}
}
