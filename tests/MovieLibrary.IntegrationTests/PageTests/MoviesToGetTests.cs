﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.IntegrationTests.Internal;
using static MovieLibrary.IntegrationTests.CustomWebApplicationFactory;

namespace MovieLibrary.IntegrationTests.PageTests
{
	[TestClass]
	public class MoviesToGetTests
	{
		[TestMethod]
		public async Task IndexPage_ForAdministratorAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task IndexPage_ForUserAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieAddingPage_ForAdministratorAccountAndMovieWithAllInfoFilled_IsLoadedCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("NewMovieToGet.MovieUri", "https://www.kinopoisk.ru/film/111543/"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles, FakeMovieInfoProvider.StubMovieInfoWithAllInfoFilled);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToGet/ConfirmMovieAdding"), formContent, CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieAddingPage_ForAdministratorAccountAndMovieWithAllInfoMissing_IsLoadedCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("NewMovieToGet.MovieUri", "https://www.kinopoisk.ru/film/13/"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles, FakeMovieInfoProvider.StubMovieInfoWithAllInfoMissing);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToGet/ConfirmMovieAdding"), formContent, CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieAddingPage_ForUserAccount_IsLoadedCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("NewMovieToGet.MovieUri", "https://www.kinopoisk.ru/film/111543/"),
			});

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles, FakeMovieInfoProvider.StubMovieInfoWithAllInfoFilled);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToGet/ConfirmMovieAdding"), formContent, CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task AddMovieAction_ForAdministratorAccountAndMovieWithAllInfoFilled_AddsMovieCorrectly()
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

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToGet/AddMovie"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/MoviesToGet", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task AddMovieAction_ForAdministratorAccountAndMovieWithAllInfoMissing_AddsMovieCorrectly()
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

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToGet/AddMovie"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/MoviesToGet", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task AddMovieAction_ForUserAccount_AddsMovieCorrectly()
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

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToGet/AddMovie"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/MoviesToGet", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task ConfirmMovingToSeePage_ForAdministratorAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet/ConfirmMovingToSee/5eac4f407a15596e90c09d7b"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovingToSeePage_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet/ConfirmMovingToSee/5eac4f407a15596e90c09d7b"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToGet%2FConfirmMovingToSee%2F5eac4f407a15596e90c09d7b"));
		}

		[TestMethod]
		public async Task ConfirmMovieDeletionPage_ForAdministratorAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet/ConfirmMovieDeletion/5eac4f407a15596e90c09d7b"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieDeletionPage_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet/ConfirmMovieDeletion/5eac4f407a15596e90c09d7b"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToGet%2FConfirmMovieDeletion%2F5eac4f407a15596e90c09d7b"));
		}

		[TestMethod]
		public async Task DeleteMovieAction_ForAdministratorAccount_DeletesMovieCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5eac4f407a15596e90c09d7b"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToGet/DeleteMovie"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/MoviesToGet", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task DeleteMovieAction_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5eac4f407a15596e90c09d7b"),
			});

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToGet/DeleteMovie"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToGet%2FDeleteMovie"));
		}
	}
}
