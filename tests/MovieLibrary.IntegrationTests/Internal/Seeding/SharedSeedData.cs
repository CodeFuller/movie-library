using System.Collections.Generic;
using System.Linq;
using MovieLibrary.Authorization;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal static class SharedSeedData
	{
		private static string AdministratorRole => "Administrator";

		private static string PrivilegedUserRole => "Privileged User";

		private static string LimitedUserRole => "Limited User";

		public static IEnumerable<RoleSeedData> ApplicationRoles
		{
			get
			{
				yield return new RoleSeedData
				{
					Id = "5eb995ec4083c272a80ca306",
					RoleName = SecurityConstants.AdministratorRole,
					Permissions = Enumerable.Empty<string>(),
				};

				yield return new RoleSeedData
				{
					Id = "5eb995ec4083c272a80ca307",
					RoleName = PrivilegedUserRole,
					Permissions = new[]
					{
						ApplicationPermissions.MoviesToGet.Add,
						ApplicationPermissions.MoviesToGet.Read,
						ApplicationPermissions.MoviesToGet.MoveToMoviesToSee,
						ApplicationPermissions.MoviesToGet.Delete,
						ApplicationPermissions.MoviesToSee.Add,
						ApplicationPermissions.MoviesToSee.Read,
						ApplicationPermissions.MoviesToSee.MarkAsSeen,
						ApplicationPermissions.MoviesToSee.Delete,
					},
				};

				yield return new RoleSeedData
				{
					Id = "5eb995ef4083c272a80ca308",
					RoleName = LimitedUserRole,
					Permissions = new[]
					{
						ApplicationPermissions.MoviesToGet.Add,
						ApplicationPermissions.MoviesToGet.Read,
						ApplicationPermissions.MoviesToSee.Read,
					},
				};
			}
		}

		public static string PrivilegedUserName => "SomeAdministrator@test.com";

		public static string LimitedUserName => "SomeLimitedUser@test.com";

		public static IEnumerable<UserSeedData> ApplicationUsers
		{
			get
			{
				yield return new UserSeedData
				{
					Id = "5eb7eb9e1fdada19f4eb59b0",
					Email = PrivilegedUserName,
					Password = "Qwerty123!",
					Roles = new[] { AdministratorRole, PrivilegedUserRole },
				};

				yield return new UserSeedData
				{
					Id = "5eb7eb9f1fdada19f4eb59b1",
					Email = LimitedUserName,
					Password = "Qwerty321!",
					Roles = new[] { LimitedUserRole },
				};
			}
		}
	}
}
