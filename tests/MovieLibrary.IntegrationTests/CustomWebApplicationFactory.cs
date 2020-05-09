using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieLibrary.IntegrationTests.Internal;
using MovieLibrary.Internal;

namespace MovieLibrary.IntegrationTests
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Startup>, IHttpClientFactory
	{
		private readonly IEnumerable<string> userRoles;

		public CustomWebApplicationFactory(IEnumerable<string> userRoles)
		{
			this.userRoles = userRoles;
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.ConfigureAppConfiguration((context, configBuilder) =>
			{
				configBuilder.AddJsonFile(GetTestRunSettingsPath(), optional: false);
			});

			builder.ConfigureServices(services =>
			{
				services.AddSingleton<IApplicationBootstrapper>(new FakeApplicationBootstrapper(userRoles));
				services.AddSingleton<IHttpClientFactory>(this);

				services.AddSingleton<IApplicationInitializer, DatabaseSeeder>();

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

		public static HttpClient CreateHttpClient(IEnumerable<string> userRoles = null)
		{
			var factory = new CustomWebApplicationFactory(userRoles);

			var options = new WebApplicationFactoryClientOptions
			{
				AllowAutoRedirect = false,
			};

			return factory.CreateClient(options);
		}

		public HttpClient CreateClient(string name)
		{
			return CreateClient();
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
