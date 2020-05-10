using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.IntegrationTests.Internal;
using static MovieLibrary.IntegrationTests.Internal.CustomWebApplicationFactory;

namespace MovieLibrary.IntegrationTests.Controllers
{
	[TestClass]
	public class MoviesToSeeControllerTests
	{
		[TestMethod]
		public async Task Index_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task Index_ForUserAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task Index_NoMoviesToSee_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles, seedData: new EmptySeedData());

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task Index_ForFirstPage_ReturnsCorrectPagingBar()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles, seedData: new PagingSeedData(), moviesPageSize: 2);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task Index_ForMiddlePage_ReturnsCorrectPagingBar()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles, seedData: new PagingSeedData(), moviesPageSize: 2);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee/page-5"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task Index_ForLastPage_ReturnsCorrectPagingBar()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles, seedData: new PagingSeedData(), moviesPageSize: 2);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee/page-10"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieAdding_ForAdministratorAccountAndMovieWithAllInfoFilled_ReturnsCorrectPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("NewMovieToSee.MovieUri", "https://www.kinopoisk.ru/film/111543/"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles, movieInfoProvider: FakeMovieInfoProvider.StubMovieInfoWithAllInfoFilled);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMovieAdding"), formContent, CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieAdding_ForAdministratorAccountAndMovieWithAllInfoMissing_ReturnsCorrectPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("NewMovieToSee.MovieUri", "https://www.kinopoisk.ru/film/13/"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles, movieInfoProvider: FakeMovieInfoProvider.StubMovieInfoWithAllInfoMissing);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMovieAdding"), formContent, CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieAdding_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("NewMovieToSee.MovieUri", "https://www.kinopoisk.ru/film/111543/"),
			});

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMovieAdding"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToSee%2FConfirmMovieAdding"));
		}

		[TestMethod]
		public async Task AddMovie_ForAdministratorAccountAndMovieWithAllInfoFilled_AddsMovieCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Title", "Темный рыцарь"),
				new KeyValuePair<string, string>("Year", "2008"),
				new KeyValuePair<string, string>("MovieUri", "https://www.kinopoisk.ru/film/111543/"),
				new KeyValuePair<string, string>("PosterUri", "https://st.kp.yandex.net/images/film_iphone/iphone360_111543.jpg"),
				new KeyValuePair<string, string>("Directors[0]", "Кристофер Нолан"),
				new KeyValuePair<string, string>("Cast[0]", "Кристиан Бэйл"),
				new KeyValuePair<string, string>("Cast[1]", "Хит Леджер"),
				new KeyValuePair<string, string>("Cast[2]", "Аарон Экхарт"),
				new KeyValuePair<string, string>("Cast[3]", "Мэгги Джилленхол"),
				new KeyValuePair<string, string>("Cast[4]", "Гари Олдман"),
				new KeyValuePair<string, string>("Cast[5]", "Майкл Кейн"),
				new KeyValuePair<string, string>("Cast[6]", "Морган Фриман"),
				new KeyValuePair<string, string>("Cast[7]", "Чинь Хань"),
				new KeyValuePair<string, string>("Cast[8]", "Нестор Карбонелл"),
				new KeyValuePair<string, string>("Cast[9]", "Эрик Робертс"),
				new KeyValuePair<string, string>("RatingValue", "8.499"),
				new KeyValuePair<string, string>("RatingVotesNumber", "467272"),
				new KeyValuePair<string, string>("Duration", "02:32:00"),
				new KeyValuePair<string, string>("Genres[0]", "фантастика"),
				new KeyValuePair<string, string>("Genres[1]", "боевик"),
				new KeyValuePair<string, string>("Genres[2]", "триллер"),
				new KeyValuePair<string, string>("Genres[3]", "криминал"),
				new KeyValuePair<string, string>("Genres[4]", "драма"),
				new KeyValuePair<string, string>("Summary", "Бэтмен поднимает ставки в войне с криминалом. С помощью лейтенанта Джима Гордона и прокурора Харви Дента он намерен очистить улицы от преступности, отравляющей город. Сотрудничество оказывается эффективным, но скоро они обнаружат себя посреди хаоса, развязанного восходящим криминальным гением, известным испуганным горожанам под именем Джокер."),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/AddMovie"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/MoviesToSee", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task AddMovie_ForAdministratorAccountAndMovieWithAllInfoMissing_AddsMovieCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Title", "New Movie Without Info"),
				new KeyValuePair<string, string>("Year", String.Empty),
				new KeyValuePair<string, string>("MovieUri", "https://www.kinopoisk.ru/film/13/"),
				new KeyValuePair<string, string>("PosterUri", String.Empty),
				new KeyValuePair<string, string>("RatingValue", String.Empty),
				new KeyValuePair<string, string>("RatingVotesNumber", String.Empty),
				new KeyValuePair<string, string>("Duration", String.Empty),
				new KeyValuePair<string, string>("Summary", String.Empty),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/AddMovie"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/MoviesToSee", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task AddMovie_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Title", "New Movie Without Info"),
				new KeyValuePair<string, string>("MovieUri", "https://www.kinopoisk.ru/film/13/"),
				new KeyValuePair<string, string>("PosterUri", String.Empty),
				new KeyValuePair<string, string>("RatingValue", String.Empty),
				new KeyValuePair<string, string>("RatingVotesNumber", String.Empty),
				new KeyValuePair<string, string>("Duration", String.Empty),
				new KeyValuePair<string, string>("Summary", String.Empty),
			});

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/AddMovie"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToSee%2FAddMovie"));
		}

		[TestMethod]
		public async Task ConfirmMarkingAsSeen_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMarkingAsSeen/5ead62d14be68246b45bba82"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMarkingAsSeen_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMarkingAsSeen/5ead62d14be68246b45bba82"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToSee%2FConfirmMarkingAsSeen%2F5ead62d14be68246b45bba82"));
		}

		[TestMethod]
		public async Task MarkMovieAsSeen_ForAdministratorAccount_DeletesMovie()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5ead62d14be68246b45bba82"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/MarkMovieAsSeen/5ead62d14be68246b45bba82"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/MoviesToSee", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task MoveToMoviesToSee_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5ead62d14be68246b45bba82"),
			});

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/MarkMovieAsSeen/5ead62d14be68246b45bba82"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToSee%2FMarkMovieAsSeen%2F5ead62d14be68246b45bba82"));
		}

		[TestMethod]
		public async Task ConfirmMovieDeletion_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMovieDeletion/5ead62d14be68246b45bba82"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieDeletion_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMovieDeletion/5ead62d14be68246b45bba82"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToSee%2FConfirmMovieDeletion%2F5ead62d14be68246b45bba82"));
		}

		[TestMethod]
		public async Task DeleteMovie_ForAdministratorAccount_DeletesMovieCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5ead62d14be68246b45bba82"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/DeleteMovie"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/MoviesToSee", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task DeleteMovie_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5ead62d14be68246b45bba82"),
			});

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/DeleteMovie"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToSee%2FDeleteMovie"));
		}
	}
}
