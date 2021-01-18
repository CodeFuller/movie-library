using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.Logic.IntegrationTests
{
	internal class DatabaseSeeder
	{
		private readonly IMoviesToGetRepository moviesToGetRepository;

		private readonly IMoviesToSeeRepository moviesToSeeRepository;

		private readonly IMongoCollection<MovieToGetDocument> moviesToGet;

		private readonly IMongoCollection<MovieToSeeDocument> moviesToSee;

		private readonly ILogger<DatabaseSeeder> logger;

		public DatabaseSeeder(IMoviesToGetRepository moviesToGetRepository, IMoviesToSeeRepository moviesToSeeRepository,
			IMongoCollection<MovieToGetDocument> moviesToGet, IMongoCollection<MovieToSeeDocument> moviesToSee, ILogger<DatabaseSeeder> logger)
		{
			this.moviesToGetRepository = moviesToGetRepository ?? throw new ArgumentNullException(nameof(moviesToGetRepository));
			this.moviesToSeeRepository = moviesToSeeRepository ?? throw new ArgumentNullException(nameof(moviesToSeeRepository));
			this.moviesToGet = moviesToGet ?? throw new ArgumentNullException(nameof(moviesToGet));
			this.moviesToSee = moviesToSee ?? throw new ArgumentNullException(nameof(moviesToSee));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task SeedData(CancellationToken cancellationToken)
		{
			await SeedMoviesToGet(cancellationToken);
			await SeedMoviesToSee(cancellationToken);
		}

		private async Task SeedMoviesToGet(CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding movies to get ...");

			foreach (var movie in DataForSeeding.MoviesToGet)
			{
				await moviesToGetRepository.AddMovie(movie, cancellationToken);
			}
		}

		private async Task SeedMoviesToSee(CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding movies to see ...");

			foreach (var movie in DataForSeeding.MoviesToSee)
			{
				await moviesToSeeRepository.AddMovie(movie, cancellationToken);
			}
		}

		public async Task ClearDatabaseData(CancellationToken cancellationToken)
		{
			await ClearCollection("moviesToGet", moviesToGet, cancellationToken);
			await ClearCollection("moviesToSee", moviesToSee, cancellationToken);
		}

		private async Task ClearCollection<TDocument>(string name, IMongoCollection<TDocument> collection, CancellationToken cancellationToken)
		{
			logger.LogInformation("Clearing collection {CollectionName} ...", name);

			await collection.DeleteManyAsync(new FilterDefinitionBuilder<TDocument>().Empty, cancellationToken);
		}
	}
}
