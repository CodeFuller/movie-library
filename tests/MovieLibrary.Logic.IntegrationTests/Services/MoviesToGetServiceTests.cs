using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
	public class MoviesToGetServiceTests
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
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			// Act

			var newMovieId = await target.AddMovie(movieInfo, CancellationToken.None);

			// Assert

			var expectedMovie = new MovieToGetModel
			{
				Id = newMovieId,
				TimestampOfAddingToGetList = new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3)),
				MovieInfo = movieInfo,
			};

			var storedMovie = target.GetAllMovies().SingleOrDefault(m => m.MovieInfo.MovieUri == movieUri);
			storedMovie.Should().BeEquivalentTo(expectedMovie);
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
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			// Act

			var newMovieId = await target.AddMovie(movieInfo, CancellationToken.None);

			// Assert

			var expectedMovie = new MovieToGetModel
			{
				Id = newMovieId,
				TimestampOfAddingToGetList = new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3)),
				MovieInfo = movieInfo,
			};

			var storedMovie = target.GetAllMovies().SingleOrDefault(m => m.MovieInfo.MovieUri == movieUri);
			storedMovie.Should().BeEquivalentTo(expectedMovie);
		}

		[TestMethod]
		public async Task GetAllMovies_SomeMoviesExist_ReturnsCorrectData()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			// Act

			var movies = target.GetAllMovies().ToList();

			// Assert

			var expectedMovies = new[]
			{
				DataForSeeding.MovieToGet1,
				DataForSeeding.MovieToGet2,
				DataForSeeding.MovieToGet3,
				DataForSeeding.MovieToGet4,
			};

			movies.Should().BeEquivalentTo(expectedMovies, x => x.WithStrictOrdering().Excluding(y => y.Id));
		}

		[TestMethod]
		public async Task GetAllMovies_NoMoviesExist_ReturnsEmptyCollection()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: false);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			// Act

			var movies = target.GetAllMovies().ToList();

			// Assert

			movies.Should().BeEmpty();
		}

		// With default serialization of DateTimeOffset to array in MongoDB, the sorting by TimestampOfAddingToGetList works incorrectly.
		// This test verifies the fix for this case.
		[TestMethod]
		public async Task GetAllMovies_PagingIsApplied_ReturnsCorrectMovies()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			// Act

			var movies = target.GetAllMovies()
				.Skip(2)
				.Take(2)
				.ToList();

			// Assert

			var expectedMovies = new[]
			{
				DataForSeeding.MovieToGet3,
				DataForSeeding.MovieToGet4,
			};

			movies.Should().BeEquivalentTo(expectedMovies, x => x.WithStrictOrdering().Excluding(y => y.Id));
		}

		[TestMethod]
		public async Task GetMovie_MovieExists_ReturnsMovieData()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			var movieId = GetMovieId(serviceProvider, DataForSeeding.MovieToGet1);

			// Act

			var movie = await target.GetMovie(movieId, CancellationToken.None);

			// Assert

			movie.Should().BeEquivalentTo(DataForSeeding.MovieToGet1, x => x.Excluding(y => y.Id));
		}

		[TestMethod]
		public async Task GetMovie_MovieDoesNotExist_ThrowsNotFoundException()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			var movieId = new MovieId("5ea68c4477f3ed42b8a798da");

			// Act

			Func<Task> call = () => target.GetMovie(movieId, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<NotFoundException>();
		}

		[TestMethod]
		public async Task MoveToMoviesToSee_MovieExists_MovesMovie()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true, StubClock(new DateTimeOffset(2022, 01, 16, 17, 30, 34, TimeSpan.FromHours(3))));
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			var movieId = GetMovieId(serviceProvider, DataForSeeding.MovieToGet1);

			// Act

			await target.MoveToMoviesToSee(movieId, CancellationToken.None);

			// Assert

			var expectedMoviesToGet = new[]
			{
				DataForSeeding.MovieToGet2,
				DataForSeeding.MovieToGet3,
				DataForSeeding.MovieToGet4,
			};

			var expectedMoviesToSee = new[]
			{
				DataForSeeding.MovieToSee1,
				DataForSeeding.MovieToSee2,
				DataForSeeding.MovieToSee3,
				DataForSeeding.MovieToSee4,
				new MovieToSeeModel
				{
					TimestampOfAddingToSeeList = new DateTimeOffset(2022, 01, 16, 17, 30, 34, TimeSpan.FromHours(3)),
					MovieInfo = DataForSeeding.MovieToGet1.MovieInfo,
				},
			};

			target.GetAllMovies().Should().BeEquivalentTo(expectedMoviesToGet, x => x.WithStrictOrdering().Excluding(y => y.Id));

			var moviesToSeeService = serviceProvider.GetRequiredService<IMoviesToSeeService>();
			moviesToSeeService.GetAllMovies().Should().BeEquivalentTo(expectedMoviesToSee, x => x.WithStrictOrdering().Excluding(y => y.Id));

			// The new id must be generated for new movies list.
			var newMovieToSee = moviesToSeeService.GetAllMovies().SingleOrDefault(m => m.MovieInfo.MovieUri == DataForSeeding.MovieToGet1.MovieInfo.MovieUri);
			newMovieToSee.Id.Should().NotBe(movieId);
		}

		[TestMethod]
		public async Task MoveToMoviesToSee_MovieDoesNotExist_ThrowsNotFoundException()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			var movieId = new MovieId("5ea68c4477f3ed42b8a798da");

			// Act

			Func<Task> call = () => target.MoveToMoviesToSee(movieId, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<NotFoundException>();
		}

		[TestMethod]
		public async Task DeleteMovie_MovieExists_DeletesMovie()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			var movieId = GetMovieId(serviceProvider, DataForSeeding.MovieToGet1);

			// Act

			await target.DeleteMovie(movieId, CancellationToken.None);

			// Assert

			var expectedMovies = new[]
			{
				DataForSeeding.MovieToGet2,
				DataForSeeding.MovieToGet3,
				DataForSeeding.MovieToGet4,
			};

			var movies = target.GetAllMovies().ToList();
			movies.Should().BeEquivalentTo(expectedMovies, x => x.WithStrictOrdering().Excluding(y => y.Id));
		}

		[TestMethod]
		public async Task DeleteMovie_MovieDoesNotExist_ThrowsNotFoundException()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			var movieId = new MovieId("5ea68c4477f3ed42b8a798da");

			// Act

			Func<Task> call = () => target.DeleteMovie(movieId, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<NotFoundException>();
		}

		private static MovieId GetMovieId(IServiceProvider serviceProvider, MovieToGetModel movie)
		{
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			return target
				.GetAllMovies()
				.Single(m => m.MovieInfo.MovieUri == movie.MovieInfo.MovieUri).Id;
		}
	}
}
