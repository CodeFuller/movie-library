using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MovieLibrary.IntegrationTests.Internal;
using MovieLibrary.Internal;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

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

				services.AddSingleton<IMovieInfoProvider>(StubMovieInfoProvider());

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

		private IMovieInfoProvider StubMovieInfoProvider()
		{
			var movieInfoProviderStub = new Mock<IMovieInfoProvider>();

			var movieInfoModel = new MovieInfoModel
			{
				Title = "Темный рыцарь",
				Year = 2008,
				MovieUri = new Uri("https://www.kinopoisk.ru/film/111543/"),
				PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_111543.jpg"),
				Directors = new[] { "Кристофер Нолан" },
				Cast = new[] { "Кристиан Бэйл", "Хит Леджер", "Аарон Экхарт", "Мэгги Джилленхол", "Гари Олдман", "Майкл Кейн", "Морган Фриман", "Чинь Хань", "Нестор Карбонелл", "Эрик Робертс", },
				Rating = new MovieRatingModel(8.499M, 467198),
				Duration = TimeSpan.FromMinutes(152),
				Genres = new[] { "фантастика", "боевик", "триллер", "криминал", "драма", },
				Summary = "Бэтмен поднимает ставки в войне с криминалом. С помощью лейтенанта Джима Гордона и прокурора Харви Дента он намерен очистить улицы от преступности, отравляющей город. Сотрудничество оказывается эффективным, но скоро они обнаружат себя посреди хаоса, развязанного восходящим криминальным гением, известным испуганным горожанам под именем Джокер.",
			};

			movieInfoProviderStub.Setup(x => x.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/111543/"), It.IsAny<CancellationToken>())).ReturnsAsync(movieInfoModel);

			return movieInfoProviderStub.Object;
		}
	}
}
