using System;
using System.Net;
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
		public async Task Get_ForIdentityAccountRegister_RedirectsToLoginPage()
		{
			// Arrange

			using var client = CustomWebApplicationFactory.CreateHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/Register"), CancellationToken.None);

			// Assert

			Assert.AreEqual(HttpStatusCode.MovedPermanently, response.StatusCode);
			Assert.AreEqual(new Uri("/Identity/Account/Login", UriKind.Relative), response.Headers.Location);
		}

		[TestMethod]
		public async Task Post_ToIdentityAccountRegister_RedirectsToLoginPage()
		{
			// Arrange

			using var client = CustomWebApplicationFactory.CreateHttpClient();

			using var content = new StringContent(String.Empty);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Identity/Account/Register"), content, CancellationToken.None);

			// Assert

			Assert.AreEqual(HttpStatusCode.MovedPermanently, response.StatusCode);
			Assert.AreEqual(new Uri("/Identity/Account/Login", UriKind.Relative), response.Headers.Location);
		}
	}
}
