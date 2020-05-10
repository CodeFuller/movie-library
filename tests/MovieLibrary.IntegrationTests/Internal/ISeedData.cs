using System.Collections.Generic;
using MovieLibrary.Dal.MongoDB.Documents;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal interface ISeedData
	{
		IEnumerable<MovieToGetDocument> MoviesToGet { get; }

		IEnumerable<MovieToSeeDocument> MoviesToSee { get; }

		IEnumerable<UserSeedData> Users { get; }
	}
}
