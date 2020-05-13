using System.Collections.Generic;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal interface ISeedData
	{
		IEnumerable<(MovieId id, MovieInfoModel movieInfo)> MoviesToGet { get; }

		IEnumerable<(MovieId id, MovieInfoModel movieInfo)> MoviesToSee { get; }

		IEnumerable<RoleSeedData> Roles { get; }

		IEnumerable<UserSeedData> Users { get; }
	}
}
