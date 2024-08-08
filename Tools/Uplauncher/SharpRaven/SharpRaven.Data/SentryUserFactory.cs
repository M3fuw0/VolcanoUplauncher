using System;
using System.Security.Principal;
using SharpRaven.Utilities;

namespace SharpRaven.Data
{
	public class SentryUserFactory : ISentryUserFactory
	{
		public SentryUser Create()
		{
			SentryUser user;
			if (!SentryRequestFactory.HasHttpContext)
			{
				user = new SentryUser(Environment.UserName);
			}
			else
			{
				SentryUser sentryUser = new SentryUser(GetPrincipal());
				sentryUser.IpAddress = GetIpAddress();
				user = sentryUser;
			}
			return OnCreate(user);
		}

		protected virtual SentryUser OnCreate(SentryUser user)
		{
			return user;
		}

		private static dynamic GetIpAddress()
		{
			try
			{
				return SentryRequestFactory.HttpContext.Request.UserHostAddress;
			}
			catch (Exception exception)
			{
				SystemUtil.WriteError(exception);
			}
			return null;
		}

		private static IPrincipal GetPrincipal()
		{
			try
			{
				return SentryRequestFactory.HttpContext.User as IPrincipal;
			}
			catch (Exception exception)
			{
				SystemUtil.WriteError(exception);
			}
			return null;
		}
	}
}
