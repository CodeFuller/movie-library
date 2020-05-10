using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.IntegrationTests.Internal;
using static MovieLibrary.IntegrationTests.Internal.CustomWebApplicationFactory;

namespace MovieLibrary.IntegrationTests
{
	[TestClass]
	public class IdentityTests
	{
		[TestMethod]
		public async Task Get_ForIdentityAccountRegister_RedirectsToLoginPage()
		{
			// Arrange

			using var client = CreateHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/Register"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyMovedPermanently(response, new Uri("/Identity/Account/Login", UriKind.Relative));
		}

		[TestMethod]
		public async Task Post_ToIdentityAccountRegister_RedirectsToLoginPage()
		{
			// Arrange

			using var client = CreateHttpClient();

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

			using var client = CreateHttpClient(userRoles: null);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/Login"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}
	}
}
