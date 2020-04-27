using System;
using System.Linq;
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
	public class MoviesToSeeServiceTests
	{
		[TestMethod]
		public async Task GetAllMovies_SomeMoviesExist_ReturnsCorrectData()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			// Act

			var movies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

			// Assert

			var sortedMovies = movies.OrderBy(m => m.TimestampOfAddingToSeeList).ToList();

			Assert.AreEqual(2, sortedMovies.Count);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee1, sortedMovies[0]);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee2, sortedMovies[1]);
		}

		[TestMethod]
		public async Task GetAllMovies_NoMoviesExist_ReturnsEmptyCollection()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: false);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			// Act

			var movies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

			// Assert

			CollectionAssert.AreEqual(Array.Empty<MovieToSeeModel>(), movies);
		}
	}
}
