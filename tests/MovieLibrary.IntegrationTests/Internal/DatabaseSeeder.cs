using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Internal;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class DatabaseSeeder : IApplicationInitializer
	{
		private readonly ISeedData seedData;

		private readonly IMongoCollection<MovieToGetDocument> moviesToGetDocumentCollection;
		private readonly IMongoCollection<MovieToSeeDocument> moviesToSeeDocumentCollection;

		public DatabaseSeeder(ISeedData seedData, IMongoCollection<MovieToGetDocument> moviesToGetDocumentCollection, IMongoCollection<MovieToSeeDocument> moviesToSeeDocumentCollection)
		{
			this.seedData = seedData ?? throw new ArgumentNullException(nameof(seedData));
			this.moviesToGetDocumentCollection = moviesToGetDocumentCollection ?? throw new ArgumentNullException(nameof(moviesToGetDocumentCollection));
			this.moviesToSeeDocumentCollection = moviesToSeeDocumentCollection ?? throw new ArgumentNullException(nameof(moviesToSeeDocumentCollection));
		}

		public async Task Initialize(CancellationToken cancellationToken)
		{
			await SeedMoviesToGet(cancellationToken);
			await SeedMoviesToSee(cancellationToken);
		}

		private async Task SeedMoviesToGet(CancellationToken cancellationToken)
		{
			await ClearCollection(moviesToGetDocumentCollection, cancellationToken);
			await InsertDocuments(moviesToGetDocumentCollection, seedData.MoviesToGet, cancellationToken);
		}

		private async Task SeedMoviesToSee(CancellationToken cancellationToken)
		{
			await ClearCollection(moviesToSeeDocumentCollection, cancellationToken);
			await InsertDocuments(moviesToSeeDocumentCollection, seedData.MoviesToSee, cancellationToken);
		}

		private static async Task ClearCollection<TDocument>(IMongoCollection<TDocument> collection, CancellationToken cancellationToken)
		{
			await collection.DeleteManyAsync(new FilterDefinitionBuilder<TDocument>().Empty, cancellationToken);
		}

		private static async Task InsertDocuments<TDocument>(IMongoCollection<TDocument> collection, IEnumerable<TDocument> documents, CancellationToken cancellationToken)
		{
			var insertOptions = new InsertManyOptions
			{
				BypassDocumentValidation = false,
			};

			await collection.InsertManyAsync(documents, insertOptions, cancellationToken);
		}
	}
}
