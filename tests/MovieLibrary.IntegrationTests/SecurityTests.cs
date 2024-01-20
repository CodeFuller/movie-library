using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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

			await using var webApplicationFactory = new CustomWebApplicationFactory();
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("http://localhost:5000/Identity/Account/Login"), CancellationToken.None);

			// Assert

			response.StatusCode.Should().Be(HttpStatusCode.TemporaryRedirect);
			response.Headers.Location.Should().Be(new Uri("https://localhost:5001/Identity/Account/Login"));
		}
	}
}
