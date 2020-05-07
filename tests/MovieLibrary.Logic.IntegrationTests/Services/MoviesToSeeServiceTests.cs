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

			var allMovies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

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

			var allMovies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

			var storedMovie = allMovies.SingleOrDefault(m => m?.MovieInfo.MovieUri == movieUri);
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
		public async Task GetMovie_MovieExists_ReturnsMovieData()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var movieId = await GetMovieId(serviceProvider, DataForSeeding.MovieToSee1);

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

		[TestMethod]
		public async Task DeleteMovie_MovieExists_DeletesMovie()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var movieId = await GetMovieId(serviceProvider, DataForSeeding.MovieToSee1);

			// Act

			await target.DeleteMovie(movieId, CancellationToken.None);

			// Assert

			var movies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

			Assert.AreEqual(1, movies.Count);
			MovieAssert.AreEqual(DataForSeeding.MovieToSee2, movies[0]);
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

		private static async Task<MovieId> GetMovieId(IServiceProvider serviceProvider, MovieToSeeModel movie)
		{
			var target = serviceProvider.GetRequiredService<IMoviesToSeeService>();

			var allMovies = await target.GetAllMovies(CancellationToken.None).ToListAsync();
			return allMovies.Single(m => m.MovieInfo.MovieUri == movie.MovieInfo.MovieUri).Id;
		}
	}
}
