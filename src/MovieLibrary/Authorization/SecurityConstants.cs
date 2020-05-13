namespace MovieLibrary.Authorization
{
	internal static class SecurityConstants
	{
		public const string AdministratorRole = "Administrator";

		public static string PermissionClaimType => "permission";

		public static string DefaultAdministratorEmail => "DefaultAdmin@localhost";

		public static string DefaultAdministratorPassword => "Qwerty123!";
	}
}
