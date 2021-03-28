using System.Collections.Generic;
using System.Linq;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal class EmptySeedData : ISeedData
	{
		public IEnumerable<(MovieId Id, MovieInfoModel MovieInfo)> MoviesToGet => Enumerable.Empty<(MovieId Id, MovieInfoModel MovieInfo)>();

		public IEnumerable<(MovieId Id, MovieInfoModel MovieInfo)> MoviesToSee => Enumerable.Empty<(MovieId Id, MovieInfoModel MovieInfo)>();

		public IEnumerable<RoleSeedData> Roles => Enumerable.Empty<RoleSeedData>();

		public IEnumerable<UserSeedData> Users => Enumerable.Empty<UserSeedData>();
	}
}
