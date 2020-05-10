using System.Collections.Generic;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal interface ISeedData
	{
		IEnumerable<(MovieId id, MovieInfoModel movieInfo)> MoviesToGet { get; }

		IEnumerable<(MovieId id, MovieInfoModel movieInfo)> MoviesToSee { get; }

		IEnumerable<UserSeedData> Users { get; }
	}
}
