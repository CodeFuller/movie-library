﻿using MovieLibrary.Authorization;
using MovieLibrary.IntegrationTests.Internal.Seeding;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class ApplicationUser
	{
		public string Name { get; }

		public static ApplicationUser PrivilegedUser => new ApplicationUser(SharedSeedData.PrivilegedUserName);

		public static ApplicationUser LimitedUser => new ApplicationUser(SharedSeedData.LimitedUserName);

		public static ApplicationUser DefaultAdministrator => new ApplicationUser(SecurityConstants.DefaultAdministratorEmail);

		private ApplicationUser(string userName)
		{
			Name = userName;
		}
	}
}
