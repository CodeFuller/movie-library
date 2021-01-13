using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.IntegrationTests.Internal;

namespace MovieLibrary.IntegrationTests.Controllers
{
	[TestClass]
	public class HomeControllerTests
	{
		[TestMethod]
		public async Task Index_ForUnauthenticatedUser_RedirectsToLoginPage()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(authenticatedUser: null);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/Login?ReturnUrl=%2F"));
		}

		[TestMethod]
		public async Task Index_ForPrivilegedUser_RedirectsToPageWithMoviesToSee()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/MoviesToSee", UriKind.Relative));
		}

		[TestMethod]
		public async Task Index_ForLimitedUser_RedirectsToPageWithMoviesToSee()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.LimitedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/MoviesToSee", UriKind.Relative));
		}
	}
}
