using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MovieLibrary.Internal;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class CustomWebApplicationFactory : WebApplicationFactory<Startup>
	{
		private readonly IEnumerable<string> userRoles;

		private readonly ISeedData seedData;

		private readonly int? moviesPageSize;

		private readonly Func<IMovieInfoProvider> fakeMovieInfoProviderFactory;

		public CustomWebApplicationFactory(IEnumerable<string> userRoles = null, ISeedData seedData = null, int? moviesPageSize = null, Func<IMovieInfoProvider> movieInfoProvider = null)
		{
			this.userRoles = userRoles;
			this.seedData = seedData ?? new DefaultSeedData();
			this.moviesPageSize = moviesPageSize;
			this.fakeMovieInfoProviderFactory = movieInfoProvider ?? FakeMovieInfoProvider.StubFailingProvider;
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.ConfigureAppConfiguration((context, configBuilder) =>
			{
				configBuilder.AddJsonFile(GetTestRunSettingsPath(), optional: false);

				if (moviesPageSize != null)
				{
					configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("moviesPageSize", moviesPageSize.Value.ToString(CultureInfo.InvariantCulture)) });
				}
			});

			builder.ConfigureServices(services =>
			{
				services.AddSingleton<IApplicationBootstrapper>(new FakeApplicationBootstrapper(userRoles));

				services.AddSingleton<ISeedData>(seedData);
				services.AddScoped<IApplicationInitializer, DatabaseSeeder>();
				services.AddSingleton<IMovieInfoProvider>(fakeMovieInfoProviderFactory());

				// Same instance should be registered for IIdGenerator and IFakeIdGenerator
				services.AddSingleton<FakeIdGenerator<ObjectId>>();
				services.AddSingleton<IIdGenerator<ObjectId>>(sp => sp.GetRequiredService<FakeIdGenerator<ObjectId>>());
				services.AddSingleton<IFakeIdGenerator<ObjectId>>(sp => sp.GetRequiredService<FakeIdGenerator<ObjectId>>());

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

		public static HttpClient CreateHttpClient(IEnumerable<string> userRoles = null, ISeedData seedData = null, int? moviesPageSize = null, Func<IMovieInfoProvider> movieInfoProvider = null)
		{
			var factory = new CustomWebApplicationFactory(userRoles, seedData, moviesPageSize, movieInfoProvider);
			return factory.CreateHttpClient();
		}

		public HttpClient CreateHttpClient()
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
