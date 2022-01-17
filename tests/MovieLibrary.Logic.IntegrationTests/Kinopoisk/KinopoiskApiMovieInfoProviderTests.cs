using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Kinopoisk;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.IntegrationTests.Kinopoisk
{
	[TestClass]
	public class KinopoiskApiMovieInfoProviderTests
	{
		[TestMethod]
		public async Task GetMovieInfo_IfAllPropertiesFilled_LoadsMovieInfoCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/342/"), CancellationToken.None);

			// Assert

			var expectedMovieInfo = new MovieInfoModel
			{
				Title = "Криминальное чтиво",
				Year = 1994,
				MovieUri = new Uri("https://www.kinopoisk.ru/film/342/"),
				PosterUri = new Uri("https://avatars.mds.yandex.net/get-kinopoisk-image/1900788/87b5659d-a159-4224-9bff-d5a5d109a53b/x1000"),
				Directors = new[] { "Квентин Тарантино" },
				Cast = new[] { "Джон Траволта", "Сэмюэл Л. Джексон", "Брюс Уиллис" },
				Duration = TimeSpan.FromMinutes(154),
				Genres = new[] { "криминал", "драма" },
				Countries = new[] { "США" },
				SummaryParagraphs = new[]
				{
					"Двое бандитов Винсент Вега и Джулс Винфилд ведут философские беседы в перерывах между разборками и решением проблем с должниками криминального босса Марселласа Уоллеса.",
					"В первой истории Винсент проводит незабываемый вечер с женой Марселласа Мией. Во второй рассказывается о боксёре Бутче Кулидже, купленном Уоллесом, чтобы сдать бой. В третьей истории Винсент и Джулс по нелепой случайности попадают в неприятности.",
				},
			};

			movieInfo.Should().BeEquivalentTo(expectedMovieInfo, x => x.WithStrictOrdering().Excluding(y => y.Rating));

			// Tricky check for rating because it's subject for change.
			var rating = movieInfo.Rating;
			rating.Value.Should().BeInRange(8.0M, 9.0M);
			rating.VotesNumber.Should().BeInRange(400_000, 10_000_000);
		}

		[TestMethod]
		public async Task GetMovieInfo_IfSomePropertiesAreMissing_LoadsMovieInfoCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/1359229/"), CancellationToken.None);

			// Assert

			var expectedMovieInfo = new MovieInfoModel
			{
				Title = "Micke Dubois: Mycket mer än Svullo",
				Year = 2006,
				MovieUri = new Uri("https://www.kinopoisk.ru/film/1359229/"),
				PosterUri = null,
				Directors = Array.Empty<string>(),
				Cast = new[] { "Ганс Криспин", "Мике Дюбуа" },
				Rating = null,
				Duration = null,
				Genres = new[] { "комедия" },
				Countries = new[] { "Швеция" },
				SummaryParagraphs = null,
			};

			movieInfo.Should().BeEquivalentTo(expectedMovieInfo, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task GetMovieInfo_IfRatingVoteCountHasNumberType_LoadsMovieInfoCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/282395/"), CancellationToken.None);

			// Assert

			movieInfo.Rating.Should().NotBeNull();
			movieInfo.Rating.VotesNumber.Should().BeLessThan(1000);
		}

		[TestMethod]
		public async Task GetMovieInfo_IfCastContainsActorsInEnglish_LoadsMovieInfoCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/1313568/"), CancellationToken.None);

			// Assert

			var expectedCast = new[]
			{
				"Чжоу Эньлай",
				"Valko Chervenkov",
				"Yumyaagiin Tsedenbal",
			};

			movieInfo.Cast.Should().Equal(expectedCast);
		}

		private static IMovieInfoProvider CreateTestTarget()
		{
			var services = new ServiceCollection();
			services.AddKinopoiskMovieInfoProvider();
			var serviceProvider = services.BuildServiceProvider();

			return serviceProvider.GetRequiredService<IMovieInfoProvider>();
		}
	}
}
