using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using MovieLibrary.Dal.MongoDB;
using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Logic.Extensions;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.Logic.IntegrationTests
{
	internal static class TestsBootstrapper
	{
		public static async Task<IServiceProvider> BootstrapTests(bool seedData, Action<IServiceCollection> servicesSetup = null)
		{
			void SetupServices(IServiceCollection services)
			{
				services.AddSingleton<IMovieInfoProvider>(Mock.Of<IMovieInfoProvider>());

				servicesSetup?.Invoke(services);
			}

			return await PrepareServiceProvider(SetupServices, seedData);
		}

		private static async Task<IServiceProvider> PrepareServiceProvider(Action<IServiceCollection> servicesSetup, bool seedData)
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("TestsSettings.json", optional: false)
				.AddEnvironmentVariables()
				.Build();

			var services = new ServiceCollection()
				.AddBusinessLogic()
				.AddMongoDbDal(configuration.GetMovieLibraryConnectionString());

			servicesSetup(services);

			var serviceProvider = services.BuildServiceProvider();

			await PrepareDatabaseForTest(serviceProvider, seedData, CancellationToken.None);

			return serviceProvider;
		}

		private static async Task PrepareDatabaseForTest(IServiceProvider serviceProvider, bool seedData, CancellationToken cancellationToken)
		{
			await ClearDatabaseData(serviceProvider, cancellationToken);

			if (seedData)
			{
				await SeedData(serviceProvider, cancellationToken);
			}
		}

		private static async Task ClearDatabaseData(IServiceProvider serviceProvider, CancellationToken cancellationToken)
		{
			await ClearCollection<MovieToGetDocument>(serviceProvider, cancellationToken);
			await ClearCollection<MovieToSeeDocument>(serviceProvider, cancellationToken);
		}

		private static async Task ClearCollection<TDocument>(IServiceProvider serviceProvider, CancellationToken cancellationToken)
		{
			var collection = serviceProvider.GetRequiredService<IMongoCollection<TDocument>>();
			await collection.DeleteManyAsync(new FilterDefinitionBuilder<TDocument>().Empty, cancellationToken);
		}

		private static async Task SeedData(IServiceProvider serviceProvider, CancellationToken cancellationToken)
		{
			await SeedMoviesToGet(serviceProvider, cancellationToken);
			await SeedMoviesToSee(serviceProvider, cancellationToken);
		}

		private static async Task SeedMoviesToSee(IServiceProvider serviceProvider, CancellationToken cancellationToken)
		{
			var repository = serviceProvider.GetRequiredService<IMoviesToGetRepository>();

			foreach (var movie in DataForSeeding.MoviesToGet)
			{
				await repository.AddMovie(movie, cancellationToken);
			}
		}

		private static async Task SeedMoviesToGet(IServiceProvider serviceProvider, CancellationToken cancellationToken)
		{
			var repository = serviceProvider.GetRequiredService<IMoviesToSeeRepository>();

			foreach (var movie in DataForSeeding.MoviesToSee)
			{
				await repository.AddMovie(movie, cancellationToken);
			}
		}
	}
}
