using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Logic.Exceptions;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using static MovieLibrary.Logic.IntegrationTests.MockHelpers;
using static MovieLibrary.Logic.IntegrationTests.TestsBootstrapper;

namespace MovieLibrary.Logic.IntegrationTests.Services
{
	[TestClass]
	public class MoviesToSeeServiceTests
	{
		[TestMethod]
		public async Task AddMovie_AllPropertiesAreFilled_SavesDataCorrectly()
		{
			// Arrange

			// Using fake URL to prevent requests to Kinopoisk if stub is misconfigured.
			var movieUri = new Uri("https://www.kinopoisk-test.ru/film/111543/");

			var movieInfo = new MovieInfoModel
			{
				Title = "Темный рыцарь",
				Year = 2008,
				MovieUri = movieUri,
				PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_111543.jpg"),
				Directors = new[] { "Кристофер Нолан" },
				Cast = new[] { "Кристиан Бэйл", "Хит Леджер", "Аарон Экхарт", "Мэгги Джилленхол", "Гари Олдман", },
				Rating = new MovieRatingModel(8.499M, 463508),
				Duration = TimeSpan.FromMinutes(152),
				Genres = new[] { "фантастика", "боевик", "триллер", "криминал", "драма", },
				Summary = "Бэтмен поднимает ставки в войне с криминалом...",
			};

			var serviceProvider = await BootstrapTests(seedData: true, StubClock(new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3))));
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			// Act

			var newMovieId = await target.AddMovie(movieInfo, CancellationToken.None);

			// Assert

			var allMovies = target.GetAllMovies().ToList();

			var storedMovie = allMovies.SingleOrDefault(m => m?.MovieInfo.MovieUri == movieUri);
			Assert.IsNotNull(storedMovie);

			Assert.AreEqual(storedMovie.Id, newMovieId);
			Assert.AreEqual(new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3)), storedMovie.TimestampOfAddingToSeeList);
			MovieAssert.AreEqual(movieInfo, storedMovie.MovieInfo);
		}

		[TestMethod]
		public async Task AddMovie_AllPropertiesAreMissing_SavesDataCorrectly()
		{
			// Arrange

			// Using fake URL to prevent requests to Kinopoisk if stub is misconfigured.
			var movieUri = new Uri("https://www.kinopoisk-test.ru/film/111543/");

			var movieInfo = new MovieInfoModel
			{
				Title = "Темный рыцарь",
				MovieUri = movieUri,
			};

			var serviceProvider = await BootstrapTests(seedData: true, StubClock(new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3))));
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			// Act

			var newMovieId = await target.AddMovie(movieInfo, CancellationToken.None);

			// Assert

			var storedMovie = target.GetAllMovies()
				.SingleOrDefault(m => m.MovieInfo.MovieUri == movieUri);
			Assert.IsNotNull(storedMovie);

			Assert.AreEqual(storedMovie.Id, newMovieId);
			Assert.AreEqual(new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3)), storedMovie.TimestampOfAddingToSeeList);
			MovieAssert.AreEqual(movieInfo, storedMovie.MovieInfo);
		}

		[TestMethod]
		public async Task GetAllMovies_SomeMoviesExist_ReturnsCorrectData()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			// Act

			var movies = target.GetAllMovies().ToList();

			// Assert

			Assert.AreEqual(4, movies.Count);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee1, movies[0]);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee2, movies[1]);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee3, movies[2]);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee4, movies[3]);
		}

		[TestMethod]
		public async Task GetAllMovies_NoMoviesExist_ReturnsEmptyCollection()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: false);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			// Act

			var movies = target.GetAllMovies().ToList();

			// Assert

			CollectionAssert.AreEqual(Array.Empty<MovieToSeeModel>(), movies);
		}

		// With default serialization of DateTimeOffset to array in MongoDB, the sorting by TimestampOfAddingToSeeList works incorrectly.
		// This test verifies the fix for this case.
		[TestMethod]
		public async Task GetAllMovies_PagingIsApplied_ReturnsCorrectMovies()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			// Act

			var movies = target.GetAllMovies()
				.Skip(2)
				.Take(2)
				.ToList();

			// Assert

			Assert.AreEqual(2, movies.Count);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee3, movies[0]);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee4, movies[1]);
		}

		[TestMethod]
		public async Task GetMovie_MovieExists_ReturnsMovieData()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var movieId = GetMovieId(serviceProvider, DataForSeeding.MovieToSee1);

			// Act

			var movie = await target.GetMovie(movieId, CancellationToken.None);

			// Assert

			MovieAssert.AreEqual(DataForSeeding.MovieToSee1, movie);
		}

		[TestMethod]
		public async Task GetMovie_MovieDoesNotExist_ThrowsNotFoundException()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var movieId = new MovieId("5ea68c4477f3ed42b8a798da");

			// Act

			Task Call() => target.GetMovie(movieId, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<NotFoundException>(Call);
		}

		[TestMethod]
		public async Task MarkMovieAsSeen_MovieExists_DeletesMovie()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var movieId = GetMovieId(serviceProvider, DataForSeeding.MovieToSee1);

			// Act

			await target.MarkMovieAsSeen(movieId, CancellationToken.None);

			// Assert

			var movies = target.GetAllMovies().ToList();

			Assert.AreEqual(3, movies.Count);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee2, movies[0]);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee3, movies[1]);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee4, movies[2]);
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

		[TestMethod]
		public async Task DeleteMovie_MovieExists_DeletesMovie()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var movieId = GetMovieId(serviceProvider, DataForSeeding.MovieToSee1);

			// Act

			await target.DeleteMovie(movieId, CancellationToken.None);

			// Assert

			var movies = target.GetAllMovies().ToList();

			Assert.AreEqual(3, movies.Count);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee2, movies[0]);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee3, movies[1]);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee4, movies[2]);
		}

		[TestMethod]
		public async Task DeleteMovie_MovieDoesNotExist_ThrowsNotFoundException()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var movieId = new MovieId("5ea68c4477f3ed42b8a798da");

			// Act

			Task Call() => target.DeleteMovie(movieId, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<NotFoundException>(Call);
		}

		private static MovieId GetMovieId(IServiceProvider serviceProvider, MovieToSeeModel movie)
		{
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			return target
				.GetAllMovies()
				.Single(m => m.MovieInfo.MovieUri == movie.MovieInfo.MovieUri).Id;
		}
	}
}
