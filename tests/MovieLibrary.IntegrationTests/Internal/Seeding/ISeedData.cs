using System.Collections.Generic;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal interface ISeedData
	{
		IEnumerable<(MovieId Id, MovieInfoModel MovieInfo, string Reference)> MoviesToGet { get; }

		IEnumerable<(MovieId Id, MovieInfoModel MovieInfo, string Reference)> MoviesToSee { get; }

		IEnumerable<RoleSeedData> Roles { get; }

		IEnumerable<UserSeedData> Users { get; }
	}
}
