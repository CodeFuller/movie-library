using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Kinopoisk;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
			Assert.AreEqual(new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_342.jpg"), movieInfo.PosterUri);
			CollectionAssert.AreEqual(new[] { "Квентин Тарантино" }, movieInfo.Directors.ToList());

			var expectedCast = new[]
			{
				"Джон Траволта", "Сэмюэл Л. Джексон", "Брюс Уиллис", "Ума Турман", "Винг Реймз", "Тим Рот",
				"Харви Кейтель", "Квентин Тарантино", "Питер Грин", "Аманда Пламмер",
			};

			CollectionAssert.AreEqual(expectedCast, movieInfo.Cast.ToList());

			// Tricky check for rating because it's subject for change.
			var rating = movieInfo.Rating;
			Assert.IsTrue(rating.Value >= 8.0M && rating.Value <= 9.0M);
			Assert.IsTrue(rating.VotesNumber > 400_000 && rating.VotesNumber < 10_000_000);

			Assert.AreEqual(TimeSpan.FromMinutes(154), movieInfo.Duration);
			CollectionAssert.AreEqual(new[] { "триллер", "комедия", "криминал", }, movieInfo.Genres.ToList());

			var expectedSummary =
				"Двое бандитов Винсент Вега и Джулс Винфилд ведут философские беседы в перерывах между разборками и решением проблем с должниками криминального босса Марселласа Уоллеса.\n\n"
				+
				"В первой истории Винсент проводит незабываемый вечер с женой Марселласа Мией. Во второй рассказывается о боксёре Бутче Кулидже, купленном Уоллесом, чтобы сдать бой. В третьей истории Винсент и Джулс по нелепой случайности попадают в неприятности.";

			Assert.AreEqual(expectedSummary, movieInfo.Summary);
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
