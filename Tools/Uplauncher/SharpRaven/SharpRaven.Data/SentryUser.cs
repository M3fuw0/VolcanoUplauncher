using System.Security.Principal;
using Newtonsoft.Json;

namespace SharpRaven.Data
{
	public class SentryUser
	{
		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Email { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Id { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string IpAddress { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Username { get; set; }

		public SentryUser(IPrincipal principal)
		{
			if (principal != null)
			{
				Username = principal.Identity.Name;
			}
		}

		public SentryUser(IIdentity identity)
		{
			if (identity != null)
			{
				Username = identity.Name;
			}
		}

		public SentryUser(string username)
		{
			Username = username;
		}

		public static SentryUser GetUser(ISentryUserFactory factory)
		{
			return factory?.Create();
		}
	}
}
