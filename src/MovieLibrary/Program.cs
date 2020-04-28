using System.Threading.Tasks;
using CF.Library.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MovieLibrary
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			await CreateHostBuilder(args).Build().RunAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				})
				.ConfigureLogging((hostingContext, loggingBuilder) =>
				{
					var loggingSettings = new LoggingSettings();
					var configuration = hostingContext.Configuration;
					configuration.Bind("logging", loggingSettings);

					var loggingConfiguration = new LoggingConfiguration();
					loggingConfiguration.LoadSettings(loggingSettings);
					loggingConfiguration.AddLogging(loggingBuilder);
				})
		;
	}
}
