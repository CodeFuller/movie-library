﻿using System;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MovieLibrary.Dal.MongoDB;
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
			BootstrapLogging(services, configuration);
			services.AddSingleton<DatabaseSeeder>();

			var serviceProvider = services.BuildServiceProvider();

			await PrepareDatabaseForTest(serviceProvider, seedData, CancellationToken.None);

			return serviceProvider;
		}

		private static void BootstrapLogging(IServiceCollection services, IConfiguration configuration)
		{
			var loggingSettings = new LoggingSettings();
			configuration.Bind("logging", loggingSettings);

			var loggingConfiguration = new LoggingConfiguration();
			loggingConfiguration.LoadSettings(loggingSettings);

			var loggerFactory = new LoggerFactory();
			loggingConfiguration.AddLogging(loggerFactory);

			services.AddSingleton<ILoggerFactory>(loggerFactory);
			services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
		}

		private static async Task PrepareDatabaseForTest(IServiceProvider serviceProvider, bool seedData, CancellationToken cancellationToken)
		{
			var databaseSeeder = serviceProvider.GetRequiredService<DatabaseSeeder>();

			await databaseSeeder.ClearDatabaseData(cancellationToken);

			if (seedData)
			{
				await databaseSeeder.SeedData(cancellationToken);
			}
		}
	}
}
