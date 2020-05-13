using System.Collections.Generic;
using MovieLibrary.IntegrationTests.Internal.Seeding;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class ApplicationUser
	{
		public string Name { get; private set; }

		public IEnumerable<string> Roles { get; private set; }

		public IEnumerable<string> Permissions { get; private set; }

		public static ApplicationUser PrivilegedUser => new ApplicationUser
		{
			Name = "Fake User",
			Roles = new[] { UserRoles.AdministratorRole, UserRoles.PrivilegedUserRole },
			Permissions = UserRoles.PrivilegedUserPermissions,
		};

		public static ApplicationUser LimitedUser => new ApplicationUser
		{
			Name = "Fake User",
			Roles = new[] { UserRoles.LimitedUserRole },
			Permissions = UserRoles.LimitedUserPermissions,
		};

		private ApplicationUser()
		{
		}
	}
}
