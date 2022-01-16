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

			Assert.AreEqual("Криминальное чтиво", movieInfo.Title);
			Assert.AreEqual(1994, movieInfo.Year);
			Assert.AreEqual(new Uri("https://www.kinopoisk.ru/film/342/"), movieInfo.MovieUri);
			Assert.AreEqual(new Uri("https://avatars.mds.yandex.net/get-kinopoisk-image/1900788/87b5659d-a159-4224-9bff-d5a5d109a53b/x1000"), movieInfo.PosterUri);
			CollectionAssert.AreEqual(new[] { "Квентин Тарантино" }, movieInfo.Directors?.ToList());

			var expectedCast = new[]
			{
				"Джон Траволта", "Сэмюэл Л. Джексон", "Брюс Уиллис",
			};

			CollectionAssert.AreEqual(expectedCast, movieInfo.Cast?.ToList());

			// Tricky check for rating because it's subject for change.
			var rating = movieInfo.Rating;
			Assert.IsNotNull(rating);
			Assert.IsTrue(rating.Value >= 8.0M && rating.Value <= 9.0M);
			Assert.IsTrue(rating.VotesNumber > 400_000 && rating.VotesNumber < 10_000_000);

			Assert.AreEqual(TimeSpan.FromMinutes(154), movieInfo.Duration);
			CollectionAssert.AreEqual(new[] { "криминал", "драма" }, movieInfo.Genres?.ToList());

			var expectedSummary =
				"Двое бандитов Винсент Вега и Джулс Винфилд ведут философские беседы в перерывах между разборками и решением проблем с должниками криминального босса Марселласа Уоллеса.\n\n"
				+
				"В первой истории Винсент проводит незабываемый вечер с женой Марселласа Мией. Во второй рассказывается о боксёре Бутче Кулидже, купленном Уоллесом, чтобы сдать бой. В третьей истории Винсент и Джулс по нелепой случайности попадают в неприятности.";

			Assert.AreEqual(expectedSummary, movieInfo.Summary);
		}

		[TestMethod]
		public async Task GetMovieInfo_IfSomePropertiesAreMissing_LoadsMovieInfoCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/1359229/"), CancellationToken.None);

			// Assert

			Assert.IsNull(movieInfo.PosterUri);
			Assert.IsFalse(movieInfo.Directors.Any());
			Assert.IsNull(movieInfo.Rating);
			Assert.IsNull(movieInfo.Duration);
			Assert.IsNull(movieInfo.Summary);
		}

		[TestMethod]
		public async Task GetMovieInfo_IfRatingVoteCountHasNumberType_LoadsMovieInfoCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var movieInfo = await target.GetMovieInfo(new Uri("https://www.kinopoisk.ru/film/282395/"), CancellationToken.None);

			// Assert

			var rating = movieInfo.Rating;
			Assert.IsNotNull(rating);
			Assert.IsNotNull(rating.VotesNumber);
			Assert.IsTrue(rating.VotesNumber.Value < 1000);
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
