using System.Collections.Generic;
using MovieLibrary.Dal.MongoDB.Documents;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal interface ISeedData
	{
		IReadOnlyCollection<MovieToGetDocument> MoviesToGet { get; }

		IReadOnlyCollection<MovieToSeeDocument> MoviesToSee { get; }
	}
}
