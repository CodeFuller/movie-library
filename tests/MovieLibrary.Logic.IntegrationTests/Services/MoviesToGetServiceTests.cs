﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieLibrary.Logic.Exceptions;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using static MovieLibrary.Logic.IntegrationTests.TestsBootstrapper;

namespace MovieLibrary.Logic.IntegrationTests.Services
{
	[TestClass]
	public class MoviesToGetServiceTests
	{
		[TestMethod]
		public async Task AddMovieByUrl_AllPropertiesAreFilled_SavesDataCorrectly()
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

			var serviceProvider = await BootstrapTests(seedData: true, StubMovieInfoProviderAndClock(movieUri, movieInfo, new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3))));
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			// Act

			await target.AddMovieByUrl(movieUri, CancellationToken.None);

			// Assert

			var allMovies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

			var storedMovie = allMovies.SingleOrDefault(m => m?.MovieInfo.MovieUri == movieUri);
			Assert.IsNotNull(storedMovie);

			Assert.IsNotNull(storedMovie.Id);
			Assert.AreEqual(new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3)), storedMovie.TimestampOfAddingToGetList);
			MovieAssert.AreEqual(movieInfo, storedMovie.MovieInfo);
		}

		[TestMethod]
		public async Task AddMovieByUrl_AllPropertiesAreMissing_SavesDataCorrectly()
		{
			// Arrange

			// Using fake URL to prevent requests to Kinopoisk if stub is misconfigured.
			var movieUri = new Uri("https://www.kinopoisk-test.ru/film/111543/");

			var movieInfo = new MovieInfoModel
			{
				Title = "Темный рыцарь",
				MovieUri = movieUri,
			};

			var serviceProvider = await BootstrapTests(seedData: true, StubMovieInfoProviderAndClock(movieUri, movieInfo, new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3))));
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			// Act

			await target.AddMovieByUrl(movieUri, CancellationToken.None);

			// Assert

			var allMovies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

			var storedMovie = allMovies.SingleOrDefault(m => m?.MovieInfo.MovieUri == movieUri);
			Assert.IsNotNull(storedMovie);

			Assert.IsNotNull(storedMovie.Id);
			Assert.AreEqual(new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3)), storedMovie.TimestampOfAddingToGetList);
			MovieAssert.AreEqual(movieInfo, storedMovie.MovieInfo);
		}

		[TestMethod]
		public async Task GetAllMovies_SomeMoviesExist_ReturnsCorrectData()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			// Act

			var movies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

			// Assert

			var sortedMovies = movies.OrderBy(m => m.TimestampOfAddingToGetList).ToList();

			Assert.AreEqual(2, sortedMovies.Count);
			MovieAssert.AreEqual(DataForSeeding.MovieToGet1, sortedMovies[0]);
			MovieAssert.AreEqual(DataForSeeding.MovieToGet2, sortedMovies[1]);
		}

		[TestMethod]
		public async Task GetAllMovies_NoMoviesExist_ReturnsEmptyCollection()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: false);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			// Act

			var movies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

			// Assert

			CollectionAssert.AreEqual(Array.Empty<MovieToGetModel>(), movies);
		}

		[TestMethod]
		public async Task MoveToMoviesToSee_MovieExists_MovesMovie()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true, StubClock(new DateTimeOffset(2020, 04, 27, 11, 41, 27, TimeSpan.FromHours(3))));
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			var movieId = await GetMovieId(serviceProvider, DataForSeeding.MovieToGet1);

			// Act

			await target.MoveToMoviesToSee(movieId, CancellationToken.None);

			// Assert

			var movies = await target.GetAllMovies(CancellationToken.None).ToListAsync();

			var oldMovie = movies.SingleOrDefault(m => m.Id == movieId);
			Assert.IsNull(oldMovie);

			// Checking that movie presents among movies to see.
			var moviesToSeeService = serviceProvider.GetRequiredService<IMoviesToSeeService>();
			var allMoviesToSee = await moviesToSeeService.GetAllMovies(CancellationToken.None).ToListAsync();
			var movieToSee = allMoviesToSee.SingleOrDefault(m => m.MovieInfo.MovieUri == DataForSeeding.MovieToGet1.MovieInfo.MovieUri);
			Assert.IsNotNull(movieToSee);

			// The new id must be generated for new movies list.
			Assert.AreNotEqual(movieId, movieToSee.Id);
			Assert.AreEqual(new DateTimeOffset(2020, 04, 27, 11, 41, 27, TimeSpan.FromHours(3)), movieToSee.TimestampOfAddingToSeeList);
			MovieAssert.AreEqual(DataForSeeding.MovieToGet1.MovieInfo, movieToSee.MovieInfo);
		}

		[TestMethod]
		public async Task MoveToMoviesToSee_MovieDoesNotExist_ThrowsNotFoundException()
		{
			// Arrange

			var serviceProvider = await BootstrapTests(seedData: true);
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			var movieId = new MovieId("5ea68c4477f3ed42b8a798da");

			// Act

			Task Call() => target.MoveToMoviesToSee(movieId, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<NotFoundException>(Call);
		}

		private static Action<IServiceCollection> StubMovieInfoProviderAndClock(Uri movieUri, MovieInfoModel movieInfo, DateTimeOffset currentTime)
		{
			var servicesSetups = new[]
			{
				StubMovieInfoProvider(movieUri, movieInfo),
				StubClock(currentTime),
			};

			return services =>
			{
				foreach (var serviceSetup in servicesSetups)
				{
					serviceSetup(services);
				}
			};
		}

		private static Action<IServiceCollection> StubMovieInfoProvider(Uri movieUri, MovieInfoModel movieInfo)
		{
			var movieInfoProviderStub = new Mock<IMovieInfoProvider>();
			movieInfoProviderStub.Setup(x => x.GetMovieInfo(movieUri, It.IsAny<CancellationToken>())).ReturnsAsync(movieInfo);

			return services => services.AddSingleton<IMovieInfoProvider>(movieInfoProviderStub.Object);
		}

		private static Action<IServiceCollection> StubClock(DateTimeOffset currentTime)
		{
			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(currentTime);

			return services => services.AddSingleton<IClock>(clockStub.Object);
		}

		private static async Task<MovieId> GetMovieId(IServiceProvider serviceProvider, MovieToGetModel movie)
		{
			var target = serviceProvider.GetRequiredService<IMoviesToGetService>();

			var allMovies = await target.GetAllMovies(CancellationToken.None).ToListAsync();
			return allMovies.Single(m => m.MovieInfo.MovieUri == movie.MovieInfo.MovieUri).Id;
		}
	}
}