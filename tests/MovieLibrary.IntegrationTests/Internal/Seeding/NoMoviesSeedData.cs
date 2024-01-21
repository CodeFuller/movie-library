using System.Collections.Generic;
using System.Linq;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal sealed class NoMoviesSeedData : ISeedData
	{
		public IEnumerable<(MovieId Id, MovieInfoModel MovieInfo, string Reference)> MoviesToGet => Enumerable.Empty<(MovieId Id, MovieInfoModel MovieInfo, string Reference)>();

		public IEnumerable<(MovieId Id, MovieInfoModel MovieInfo, string Reference)> MoviesToSee => Enumerable.Empty<(MovieId Id, MovieInfoModel MovieInfo, string Reference)>();

		public IEnumerable<RoleSeedData> Roles => SharedSeedData.ApplicationRoles;

		public IEnumerable<UserSeedData> Users => SharedSeedData.ApplicationUsers;
	}
}
