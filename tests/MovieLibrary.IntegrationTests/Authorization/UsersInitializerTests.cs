using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Authorization;
using MovieLibrary.IntegrationTests.Internal;
using MovieLibrary.IntegrationTests.Internal.Seeding;
using MovieLibrary.UserManagement.Interfaces;
using static MovieLibrary.IntegrationTests.Internal.CustomWebApplicationFactory;

namespace MovieLibrary.IntegrationTests.Authorization
{
	[TestClass]
	public class UsersInitializerTests
	{
		[TestMethod]
		public async Task Initialize_ForEmptyDatabase_CreatesDefaultAdministrator()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(authenticatedUser: null, seedData: new EmptySeedData());
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/Login"), CancellationToken.None);

			// Assert

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

			using var scopeServiceProvider = webApplicationFactory.Services.CreateScope();
			var userService = scopeServiceProvider.ServiceProvider.GetRequiredService<IUserService>();

			var users = await userService.GetAllUsers(CancellationToken.None).ToListAsync();
			Assert.AreEqual(1, users.Count);
			var user = users.Single();
			Assert.AreEqual(SecurityConstants.DefaultAdministratorEmail, user.UserName);
			Assert.IsFalse(user.CanBeEdited);
			Assert.IsFalse(user.CanBeDeleted);

			var roles = await userService.GetUserRoles(user.Id, CancellationToken.None);
			CollectionAssert.AreEqual(new[] { SecurityConstants.AdministratorRole }, roles.Select(r => r.RoleName).ToList());
		}

		[TestMethod]
		public async Task Initialize_SomeAdministratorExists_DoesNotCreateAnyUsers()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(authenticatedUser: null);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/Login"), CancellationToken.None);

			// Assert

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

			using var scopeServiceProvider = webApplicationFactory.Services.CreateScope();
			var userService = scopeServiceProvider.ServiceProvider.GetRequiredService<IUserService>();

			var users = (await userService.GetAllUsers(CancellationToken.None).ToListAsync())
				.Select(u => u.UserName);

			CollectionAssert.AreEqual(new[] { SharedSeedData.PrivilegedUserName, SharedSeedData.LimitedUserName, }, users.ToList());
		}

		[TestMethod]
		public async Task Access_ForDefaultAdministratorFromLocalhost_IsAllowed()
		{
			// Arrange

			using var client = CreateHttpClient(authenticatedUser: ApplicationUser.DefaultAdministrator, seedData: new EmptySeedData());

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task Access_ForDefaultAdministratorFromRemoteHost_IsDenied()
		{
			// Arrange

			using var client = CreateHttpClient(authenticatedUser: ApplicationUser.DefaultAdministrator, seedData: new EmptySeedData(), remoteIpAddress: "77.77.77.77");

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers"));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}
	}
}
