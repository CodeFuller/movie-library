using System;
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

			await PageAssert.VerifyResponse(response);
		}

		[TestMethod]
		public async Task IndexPage_ForUserAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToSee"), CancellationToken.None);

			// Assert

			await PageAssert.VerifyResponse(response);
		}
	}
}
