using System;
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
		public async Task ConfirmMovieAddingPage_ForAdministratorAccount_IsLoadedCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("NewMovieToGet.MovieUri", "https://www.kinopoisk.ru/film/111543/"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

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

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToGet/ConfirmMovieAdding"), formContent, CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
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
