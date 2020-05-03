using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MovieLibrary.IntegrationTests
{
	[TestClass]
	public class IdentityTests
	{
		[TestMethod]
		public async Task Get_ForIdentityAccountRegister_RedirectsToLoginPage()
		{
			// Arrange

			using var client = CreateClient();

			// Act

			using var response = await client.GetAsync(new Uri("/Identity/Account/Register", UriKind.Relative), CancellationToken.None);

			// Assert

			Assert.AreEqual(HttpStatusCode.MovedPermanently, response.StatusCode);
			Assert.AreEqual(new Uri("/Identity/Account/Login", UriKind.Relative), response.Headers.Location);
		}

		[TestMethod]
		public async Task Post_ToIdentityAccountRegister_RedirectsToLoginPage()
		{
			// Arrange

			using var client = CreateClient();

			using var content = new StringContent(String.Empty);

			// Act

			using var response = await client.PostAsync(new Uri("/Identity/Account/Register", UriKind.Relative), content, CancellationToken.None);

			// Assert

			Assert.AreEqual(HttpStatusCode.MovedPermanently, response.StatusCode);
			Assert.AreEqual(new Uri("/Identity/Account/Login", UriKind.Relative), response.Headers.Location);
		}

		private static HttpClient CreateClient()
		{
			var factory = new CustomWebApplicationFactory();

			var options = new WebApplicationFactoryClientOptions
			{
				AllowAutoRedirect = false,
			};

			return factory.CreateClient(options);
		}
	}
}
