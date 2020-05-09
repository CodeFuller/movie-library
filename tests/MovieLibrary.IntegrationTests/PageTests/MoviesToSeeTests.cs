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
	public class MoviesToSeeTests
	{
		[TestMethod]
		public async Task IndexPage_ForAdministratorAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task IndexPage_ForUserAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieAddingPage_ForAdministratorAccount_IsLoadedCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("NewMovieToSee.MovieUri", "https://www.kinopoisk.ru/film/111543/"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMovieAdding"), formContent, CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieAddingPage_ForUserAccount_RedirectsToAccessDeniedPage()
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
		public async Task ConfirmMarkingAsSeenPage_ForAdministratorAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMarkingAsSeen/5ead62d14be68246b45bba82"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMarkingAsSeenPage_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMarkingAsSeen/5ead62d14be68246b45bba82"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToSee%2FConfirmMarkingAsSeen%2F5ead62d14be68246b45bba82"));
		}

		[TestMethod]
		public async Task ConfirmMovieDeletionPage_ForAdministratorAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMovieDeletion/5ead62d14be68246b45bba82"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmMovieDeletionPage_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee/ConfirmMovieDeletion/5ead62d14be68246b45bba82"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FMoviesToSee%2FConfirmMovieDeletion%2F5ead62d14be68246b45bba82"));
		}

		[TestMethod]
		public async Task DeleteMovieAction_ForAdministratorAccount_DeletesMovieCorrectly()
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
		public async Task DeleteMovieAction_ForUserAccount_RedirectsToAccessDeniedPage()
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
