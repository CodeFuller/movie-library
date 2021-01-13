using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.IntegrationTests.Internal;

namespace MovieLibrary.IntegrationTests
{
	[TestClass]
	public class ErrorHandlingTests
	{
		[TestMethod]
		public async Task ControllerAction_UnhandledExceptionIsThrown_ReturnsCorrectErrorPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("NewMovieToGet.MovieUri", "https://www.kinopoisk.ru/film/111543/"),
			});

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.LimitedUser, movieInfoProvider: FakeMovieInfoProvider.StubFailingProvider);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/MoviesToGet/ConfirmMovieAdding"), formContent, CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response, HttpStatusCode.InternalServerError);
		}

		[TestMethod]
		public async Task Request_ForUnknownPage_CorrectErrorPageIsReturned()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.LimitedUser, movieInfoProvider: FakeMovieInfoProvider.StubFailingProvider);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/NoSuchPage"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response, HttpStatusCode.NotFound);
		}
	}
}
