using System.Collections.Generic;
using MovieLibrary.Internal;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal static class UserRoles
	{
		public static IEnumerable<string> AdministratorRoles => new[]
		{
			Roles.Administrator,
			Roles.CanAddMoviesToGet,
			Roles.CanReadMoviesToGet,
			Roles.CanDeleteMoviesToGet,
			Roles.CanAddMoviesToSee,
			Roles.CanReadMoviesToSee,
			Roles.CanMarkMoviesAsSeen,
			Roles.CanDeleteMoviesToSee,
		};

		public static IEnumerable<string> LimitedUserRoles => new[]
		{
			Roles.CanAddMoviesToGet,
			Roles.CanReadMoviesToGet,
			Roles.CanReadMoviesToSee,
		};
	}
}
