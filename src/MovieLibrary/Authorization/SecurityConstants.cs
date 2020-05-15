using System;

namespace MovieLibrary.Authorization
{
	internal static class SecurityConstants
	{
		public const string AdministratorRole = "Administrator";

		public static string PermissionClaimType => "permission";

		public static string DefaultAdministratorEmail => "DefaultAdmin@localhost";

		public static string DefaultAdministratorPassword => "Qwerty123!";

		public static bool IsDefaultAdministrator(string userName)
		{
			return String.Equals(userName, DefaultAdministratorEmail, StringComparison.Ordinal);
		}

		public static bool IsAdministratorRole(string roleName)
		{
			return String.Equals(roleName, AdministratorRole, StringComparison.Ordinal);
		}
	}
}
