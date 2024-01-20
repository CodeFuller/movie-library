using System;
using System.Threading;
using Moq;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal static class FakeMovieInfoProvider
	{
		public static IMovieInfoProvider StubMovieInfoWithAllInfoFilled()
		{
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
				Countries = new[] { "США", "Великобритания", },
				SummaryParagraphs = new[]
				{
					"Бэтмен поднимает ставки в войне с криминалом. С помощью лейтенанта Джима Гордона и прокурора Харви Дента он намерен очистить улицы от преступности, отравляющей город. Сотрудничество оказывается эффективным, но скоро они обнаружат себя посреди хаоса, развязанного восходящим криминальным гением, известным испуганным горожанам под именем Джокер.",
				},
			};

			return StubMovieInfoProvider(movieInfoModel);
		}

		public static IMovieInfoProvider StubMovieInfoWithAllInfoMissing()
		{
			var movieInfoModel = new MovieInfoModel
			{
				Title = "Movie Without Any Info",
				MovieUri = new Uri("https://www.kinopoisk.ru/film/13/"),
			};

			return StubMovieInfoProvider(movieInfoModel);
		}

		private static IMovieInfoProvider StubMovieInfoProvider(MovieInfoModel movieInfoModel)
		{
			var movieInfoProviderStub = new Mock<IMovieInfoProvider>();
			movieInfoProviderStub.Setup(x => x.GetMovieInfo(movieInfoModel.MovieUri, It.IsAny<CancellationToken>())).ReturnsAsync(movieInfoModel);

			return movieInfoProviderStub.Object;
		}

		// We do not want any requests to real movie info provider from Web integration tests.
		public static IMovieInfoProvider StubFailingProvider()
		{
			var movieInfoProviderStub = new Mock<IMovieInfoProvider>();
			movieInfoProviderStub.Setup(x => x.GetMovieInfo(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
				.ThrowsAsync(new InvalidOperationException("Movie info provider should be stubbed"));

			return movieInfoProviderStub.Object;
		}
	}
}
