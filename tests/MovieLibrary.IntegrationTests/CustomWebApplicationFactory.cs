using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MovieLibrary.IntegrationTests
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Startup>, IHttpClientFactory
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureAppConfiguration((context, configBuilder) =>
			{
				configBuilder.AddJsonFile(GetTestRunSettingsPath(), optional: false);
			});

			builder.ConfigureServices(services =>
			{
				services.AddSingleton<IHttpClientFactory>(this);
			});
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
