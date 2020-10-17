using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Kinopoisk;

namespace MovieLibrary.Logic.IntegrationTests.Kinopoisk
{
	[TestClass]
	public class KinopoiskMovieInfoProviderTests
	{
		// Reusing target instance so that cookies are loaded once by KinopoiskHtmlContentProvider.
		// Using static because MSTest creates new instance of test class for each test method.
		private static IMovieInfoProvider target;

		public KinopoiskMovieInfoProviderTests()
		{
			if (target != null)
			{
				return;
			}

			var services = new ServiceCollection();
			services.AddKinopoiskMovieInfoProvider();
			var serviceProvider = services.BuildServiceProvider();

			target = serviceProvider.GetRequiredService<IMovieInfoProvider>();
		}

		[TestMethod]
		public async Task GetMovieInfo_AllPropertiesFilled_LoadsMovieInfoCorrectly()
		{
			// Arrange

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/342/"), CancellationToken.None);

			// Assert

			Assert.AreEqual("Криминальное чтиво", movieInfo.Title);
			Assert.AreEqual(1994, movieInfo.Year);
			Assert.AreEqual(new Uri("https://www.kinopoisk.ru/film/342/"), movieInfo.MovieUri);
			Assert.AreEqual(new Uri("https://avatars.mds.yandex.net/get-kinopoisk-image/1900788/87b5659d-a159-4224-9bff-d5a5d109a53b/300x450"), movieInfo.PosterUri);
			CollectionAssert.AreEqual(new[] { "Квентин Тарантино" }, movieInfo.Directors?.ToList());

			var expectedCast = new[]
			{
				"Джон Траволта", "Сэмюэл Л. Джексон", "Брюс Уиллис", "Ума Турман", "Винг Реймз", "Тим Рот",
				"Харви Кейтель", "Квентин Тарантино", "Питер Грин", "Аманда Пламмер",
			};

			CollectionAssert.AreEqual(expectedCast, movieInfo.Cast?.ToList());

			// Tricky check for rating because it's subject for change.
			var rating = movieInfo.Rating;
			Assert.IsNotNull(rating);
			Assert.IsTrue(rating.Value >= 8.0M && rating.Value <= 9.0M);
			Assert.IsTrue(rating.VotesNumber > 400_000 && rating.VotesNumber < 10_000_000);

			Assert.AreEqual(TimeSpan.FromMinutes(154), movieInfo.Duration);
			CollectionAssert.AreEqual(new[] { "триллер", "комедия", "криминал", }, movieInfo.Genres?.ToList());

			var expectedSummary =
				"Двое бандитов Винсент Вега и Джулс Винфилд ведут философские беседы в перерывах между разборками и решением проблем с должниками криминального босса Марселласа Уоллеса.\n\n"
				+
				"В первой истории Винсент проводит незабываемый вечер с женой Марселласа Мией. Во второй рассказывается о боксёре Бутче Кулидже, купленном Уоллесом, чтобы сдать бой. В третьей истории Винсент и Джулс по нелепой случайности попадают в неприятности.";

			Assert.AreEqual(expectedSummary, movieInfo.Summary);
		}

		[TestMethod]
		public async Task GetMovieInfo_MoreThanThreeDirectors_LoadsDirectorsCorrectly()
		{
			// Arrange

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/4250/"), CancellationToken.None);

			// Assert

			CollectionAssert.AreEqual(new[] { "Эллисон Андерс", "Александр Рокуэлл", "Роберт Родригес" }, movieInfo.Directors?.ToList());
		}

		[TestMethod]
		public async Task GetMovieInfo_TwoDirectors_LoadsDirectorsCorrectly()
		{
			// Arrange

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/555/"), CancellationToken.None);

			// Assert

			CollectionAssert.AreEqual(new[] { "Джоэл Коэн", "Итан Коэн" }, movieInfo.Directors?.ToList());
		}

		[TestMethod]
		public async Task GetMovieInfo_MovieWithImaxVersion_LoadsTitleCorrectly()
		{
			// Arrange

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/768561/"), CancellationToken.None);

			// Assert

			Assert.AreEqual("К звёздам", movieInfo.Title);
		}

		[TestMethod]
		public async Task GetMovieInfo_MovieInTVFormat_LoadsTitleCorrectly()
		{
			// Arrange

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/95636/"), CancellationToken.None);

			// Assert

			Assert.AreEqual("Пираты Силиконовой Долины", movieInfo.Title);
		}

		[TestMethod]
		public async Task GetMovieInfo_SomePropertiesMissing_LoadsMovieInfoCorrectly()
		{
			// Arrange

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/1359229/"), CancellationToken.None);

			// Assert

			Assert.IsNull(movieInfo.PosterUri);
			Assert.IsNull(movieInfo.Directors);
			Assert.IsNull(movieInfo.Rating);
			Assert.IsNull(movieInfo.Duration);
			Assert.IsNull(movieInfo.Summary);
		}
	}
}
