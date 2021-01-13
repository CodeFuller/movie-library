using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.IntegrationTests.Internal;

namespace MovieLibrary.IntegrationTests
{
	[TestClass]
	public class SecurityTests
	{
		[TestMethod]
		public async Task Get_ForHttp_RedirectsToHttps()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory();
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("http://localhost:5000/Identity/Account/Login"), CancellationToken.None);

			// Assert

			Assert.AreEqual(HttpStatusCode.TemporaryRedirect, response.StatusCode);
			Assert.AreEqual(new Uri("https://localhost:5001/Identity/Account/Login"), response.Headers.Location);
		}
	}
}
