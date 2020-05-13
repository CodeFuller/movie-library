using System.Collections.Generic;
using MovieLibrary.Authorization;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal static class UserRoles
	{
		public static string AdministratorRole => "Administrator";

		public static string PrivilegedUserRole => "Privileged User";

		public static string LimitedUserRole => "Limited User";

		public static IEnumerable<string> PrivilegedUserPermissions
		{
			get
			{
				yield return ApplicationPermissions.MoviesToGet.Add;
				yield return ApplicationPermissions.MoviesToGet.Read;
				yield return ApplicationPermissions.MoviesToGet.MoveToMoviesToSee;
				yield return ApplicationPermissions.MoviesToGet.Delete;
				yield return ApplicationPermissions.MoviesToSee.Add;
				yield return ApplicationPermissions.MoviesToSee.Read;
				yield return ApplicationPermissions.MoviesToSee.MarkAsSeen;
				yield return ApplicationPermissions.MoviesToSee.Delete;
			}
		}

		public static IEnumerable<string> LimitedUserPermissions
		{
			get
			{
				yield return ApplicationPermissions.MoviesToGet.Add;
				yield return ApplicationPermissions.MoviesToGet.Read;
				yield return ApplicationPermissions.MoviesToSee.Read;
			}
		}
	}
}
