using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.IntegrationTests.Internal;

namespace MovieLibrary.IntegrationTests
{
	[TestClass]
	public class IdentityTests
	{
		[TestMethod]
		public async Task Get_ForIdentityAccountRegisterAndUnauthenticatedUser_RedirectsToLoginPage()
		{
			// Arrange

			await using var webApplicationFactory = new CustomWebApplicationFactory(authenticatedUser: null);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/Register"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/Login?ReturnUrl=%2FIdentity%2FAccount%2FRegister"));
		}

		[TestMethod]
		public async Task Get_ForIdentityAccountRegisterAndAuthenticatedUser_RedirectsToLoginPage()
		{
			// Arrange

			await using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/Register"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyMovedPermanently(response, new Uri("/Identity/Account/Login", UriKind.Relative));
		}

		[TestMethod]
		public async Task Post_ToIdentityAccountRegisterAndUnauthenticatedUser_RedirectsToLoginPage()
		{
			// Arrange

			await using var webApplicationFactory = new CustomWebApplicationFactory(authenticatedUser: null);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			using var content = new StringContent(String.Empty);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Identity/Account/Register"), content, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/Login?ReturnUrl=%2FIdentity%2FAccount%2FRegister"));
		}

		[TestMethod]
		public async Task Post_ToIdentityAccountRegisterAndAuthenticatedUser_RedirectsToLoginPage()
		{
			// Arrange

			await using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			using var content = new StringContent(String.Empty);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Identity/Account/Register"), content, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyMovedPermanently(response, new Uri("/Identity/Account/Login", UriKind.Relative));
		}

		[TestMethod]
		public async Task GetLogin_ForUnauthenticatedUser_ReturnsCorrectPage()
		{
			// Arrange

			await using var webApplicationFactory = new CustomWebApplicationFactory(authenticatedUser: null);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/Login"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task Get_ToInternalPageForUnauthenticatedUser_RedirectsToLoginPage()
		{
			// Arrange

			await using var webApplicationFactory = new CustomWebApplicationFactory(authenticatedUser: null);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/Login?ReturnUrl=%2FMoviesToSee"));
		}
	}
}
