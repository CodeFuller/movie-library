using MovieLibrary.Authorization;
using MovieLibrary.IntegrationTests.Internal.Seeding;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class ApplicationUser
	{
		public string Name { get; }

		public static ApplicationUser PrivilegedUser => new(SharedSeedData.PrivilegedUserName);

		public static ApplicationUser LimitedUser => new(SharedSeedData.LimitedUserName);

		public static ApplicationUser DefaultAdministrator => new(SecurityConstants.DefaultAdministratorEmail);

		private ApplicationUser(string userName)
		{
			Name = userName;
		}
	}
}
