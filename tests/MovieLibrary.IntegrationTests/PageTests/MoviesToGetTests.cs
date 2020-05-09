using System;
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
		public async Task MoviesToGetPage_ForAdministratorAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet"), CancellationToken.None);

			// Assert

			await PageAssert.VerifyResponse(response);
		}

		[TestMethod]
		public async Task MoviesToGetPage_ForUserAccount_IsLoadedCorrectly()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/MoviesToGet"), CancellationToken.None);

			// Assert

			await PageAssert.VerifyResponse(response);
		}
	}
}
