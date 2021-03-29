using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using static MovieLibrary.Logic.IntegrationTests.TestsBootstrapper;

namespace MovieLibrary.Logic.IntegrationTests.Services
{
	[TestClass]
	public class MovieUniquenessCheckerTests
	{
		[TestMethod]
		public async Task CheckMovie_MovieIsUnique_ReturnsMovieIsUnique()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMovieUniquenessChecker>();

			// Act

			var result = await target.CheckMovie(new Uri("https://www.kinopoisk.ru/film/12345/"), CancellationToken.None);

			// Assert

			Assert.AreEqual(MovieUniquenessCheckResult.MovieIsUnique, result);
		}

		[TestMethod]
		public async Task CheckMovie_MovieExistsInMoviesToGet_ReturnsExistsInMoviesToGet()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMovieUniquenessChecker>();

			// Act

			var result = await target.CheckMovie(new Uri("https://www.kinopoisk.ru/film/777/"), CancellationToken.None);

			// Assert

			Assert.AreEqual(MovieUniquenessCheckResult.ExistsInMoviesToGet, result);
		}

		[TestMethod]
		public async Task CheckMovie_MovieExistsInMoviesToSee_ReturnsExistsInMoviesToSee()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMovieUniquenessChecker>();

			// Act

			var result = await target.CheckMovie(new Uri("https://www.kinopoisk.ru/film/888/"), CancellationToken.None);

			// Assert

			Assert.AreEqual(MovieUniquenessCheckResult.ExistsInMoviesToSee, result);
		}
	}
}
