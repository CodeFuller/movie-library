using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using AspNetCore.Identity.Mongo.Model;
using CodeFuller.Library.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MovieLibrary.IntegrationTests.Extensions;
using MovieLibrary.IntegrationTests.Internal.Seeding;
using MovieLibrary.Internal;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal sealed class CustomWebApplicationFactory : WebApplicationFactory<Startup>
	{
		private readonly ApplicationUser authenticatedUser;

		private readonly ISeedData seedData;

		private readonly int? moviesPageSize;

		private readonly Func<IMovieInfoProvider> fakeMovieInfoProviderFactory;

		public CustomWebApplicationFactory(ApplicationUser authenticatedUser = null, ISeedData seedData = null, int? moviesPageSize = null, Func<IMovieInfoProvider> movieInfoProvider = null)
		{
			this.authenticatedUser = authenticatedUser;
			this.seedData = seedData ?? new DefaultSeedData();
			this.moviesPageSize = moviesPageSize;
			this.fakeMovieInfoProviderFactory = movieInfoProvider ?? FakeMovieInfoProvider.StubFailingProvider;
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			// This is required for testing of error pages.
			builder.UseEnvironment(Environments.Production);

			builder.ConfigureAppConfiguration((_, configBuilder) =>
			{
				var jsonConfigSource = new JsonConfigurationSource
				{
					Path = GetTestRunSettingsPath(),
					Optional = false,
				};

				jsonConfigSource.ResolveFileProvider();

				// We add TestsSettings.json before environment variables source,
				// leaving a chance to override connection string via environment variables.
				configBuilder.Sources.InsertBeforeType(jsonConfigSource, typeof(EnvironmentVariablesConfigurationSource));

				if (moviesPageSize != null)
				{
					configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("moviesPageSize", moviesPageSize.Value.ToString(CultureInfo.InvariantCulture)) });
				}
			});

			builder.ConfigureLogging((hostingContext, loggingBuilder) =>
			{
				loggingBuilder.ClearProviders();
				loggingBuilder.AddLogging(settings => hostingContext.Configuration.Bind("logging", settings));
			});

			builder.ConfigureServices(services =>
			{
				services.AddSingleton<IApplicationBootstrapper>(new FakeApplicationBootstrapper<MongoUser>(authenticatedUser));

				// We insert DatabaseSeeder as first service, so that it is executed before UsersInitializer.
				services.Insert(0, ServiceDescriptor.Scoped<IApplicationInitializer, DatabaseSeeder>());

				services.AddSingleton<ISeedData>(seedData);
				services.AddSingleton<IMovieInfoProvider>(fakeMovieInfoProviderFactory());

				// Same instance should be registered for IIdGeneratorQueue and IIdGenerator.
				services.AddSingleton<FakeIdGenerator>();
				services.AddSingleton<IIdGeneratorQueue>(sp => sp.GetRequiredService<FakeIdGenerator>());
				services.AddSingleton<IIdGenerator<ObjectId>>(sp => sp.GetRequiredService<FakeIdGenerator>());

				services.AddHttpsRedirection(options =>
				{
					// By default test application listens for https requests on port 5001.
					// We must let HTTPS redirection know that the port differs from default 443,
					// otherwise HTTPS redirection will not happen.
					// https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-3.1&tabs=visual-studio#port-configuration
					options.HttpsPort = 5001;
				});
			});
		}

		public HttpClient CreateDefaultHttpClient()
		{
			var options = new WebApplicationFactoryClientOptions
			{
				AllowAutoRedirect = false,
			};

			return CreateClient(options);
		}

		private static string GetTestRunSettingsPath()
		{
			return Path.Combine(GetCurrentDirectory(), "TestsSettings.json");
		}

		private static string GetCurrentDirectory()
		{
			var currentAssembly = Assembly.GetExecutingAssembly().Location;
			return Path.GetDirectoryName(currentAssembly) ?? throw new InvalidOperationException("Failed to get current directory");
		}
	}
}
