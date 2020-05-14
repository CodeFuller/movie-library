using System.Collections.Generic;
using System.Linq;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal class EmptySeedData : ISeedData
	{
		public IEnumerable<(MovieId id, MovieInfoModel movieInfo)> MoviesToGet => Enumerable.Empty<(MovieId id, MovieInfoModel movieInfo)>();

		public IEnumerable<(MovieId id, MovieInfoModel movieInfo)> MoviesToSee => Enumerable.Empty<(MovieId id, MovieInfoModel movieInfo)>();

		public IEnumerable<RoleSeedData> Roles => Enumerable.Empty<RoleSeedData>();

		public IEnumerable<UserSeedData> Users => Enumerable.Empty<UserSeedData>();
	}
}
