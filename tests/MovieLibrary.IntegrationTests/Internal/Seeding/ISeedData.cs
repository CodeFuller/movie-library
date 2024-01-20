using System.Collections.Generic;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal interface ISeedData
	{
		IEnumerable<(MovieId Id, MovieInfoModel MovieInfo)> MoviesToGet { get; }

		IEnumerable<(MovieId Id, MovieInfoModel MovieInfo)> MoviesToSee { get; }

		IEnumerable<RoleSeedData> Roles { get; }

		IEnumerable<UserSeedData> Users { get; }
	}
}
