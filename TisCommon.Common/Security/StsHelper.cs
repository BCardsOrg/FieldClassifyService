using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Security
{
	public static class StsHelper
	{
        public const string WindowsNotAllowedMsg = "Windows Authentication is not allowed.";
        public const string UserDoesNotExist = "User does not exist.";
        public const string InvalidPassword = "Invalid password.";

		private const string Seperator = @"\";

		public static string[] SplitClaimValue(string claimValue)
		{
			int i = claimValue.IndexOf(Seperator);
			if (i < 0)
			{
				return new string[] { string.Empty, claimValue };
			}
			else
			{
				return new string[] { claimValue.Substring(0, i), claimValue.Substring(i + 1) };
			}
		}

		public static string ExtractAppName(string claimValue)
		{
			return SplitClaimValue(claimValue)[0];
		}

		public static string ExtractRoleName(string claimValue)
		{
			return SplitClaimValue(claimValue)[1];
		}

		public static bool IsAppRole(string appName, string claimValue)
		{
			string roleAppName = ExtractAppName(claimValue);

			// Is there is no application, then this role apply for all applications
			if ( (roleAppName == string.Empty) && (appName != TisServicesConst.SystemApplication) )
				return true;

			return string.Compare(appName, roleAppName, true) == 0;
		}

		public static string CombineAppAndRole( string appName, string roleName )
		{
			return appName + Seperator + roleName;
		}
	}
}
