using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Logic.Exceptions;
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

		[TestMethod]
		public async Task MarkMovieAsSeen_MovieExists_DeletesMovie()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var movieId = await GetMovieId(serviceProvider, DataForSeeding.MovieToSee1);

			// Act

			await target.MarkMovieAsSeen(movieId, CancellationToken.None);

			// Assert

			var movies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

			Assert.AreEqual(1, movies.Count);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee2, movies[0]);
		}

		[TestMethod]
		public async Task MarkMovieAsSeen_MovieDoesNotExist_ThrowsNotFoundException()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var movieId = new MovieId("5ea68c4477f3ed42b8a798da");

			// Act

			Task Call() => target.MarkMovieAsSeen(movieId, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<NotFoundException>(Call);
		}

		private static async Task<MovieId> GetMovieId(IServiceProvider serviceProvider, MovieToSeeModel movie)
		{
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var allMovies = await target.GetAllMovies(CancellationToken.None).ToListAsync();
			return allMovies.Single(m => m.MovieInfo.MovieUri == movie.MovieInfo.MovieUri).Id;
		}
	}
}
