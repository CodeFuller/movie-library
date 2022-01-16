using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Authorization;
using MovieLibrary.IntegrationTests.Internal;
using MovieLibrary.IntegrationTests.Internal.Seeding;
using MovieLibrary.UserManagement.Interfaces;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.IntegrationTests.Authorization
{
	[TestClass]
	public class UsersInitializerTests
	{
		[TestMethod]
		public async Task Initialize_ForEmptyDatabase_CreatesDefaultAdministrator()
		{
			// Arrange

			await using var webApplicationFactory = new CustomWebApplicationFactory(authenticatedUser: null, seedData: new EmptySeedData());
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/Login"), CancellationToken.None);

			// Assert

			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var expectedUsers = new[]
			{
				new UserModel
				{
					UserName = SecurityConstants.DefaultAdministratorEmail,
					CanBeEdited = false,
					CanBeDeleted = false,
				},
			};

			var expectedRoles = new[]
			{
				new RoleModel
				{
					RoleName = SecurityConstants.AdministratorRole,
					ReadOnly = true,
				},
			};

			using var scopeServiceProvider = webApplicationFactory.Services.CreateScope();
			var userService = scopeServiceProvider.ServiceProvider.GetRequiredService<IUserService>();

			var users = await userService.GetAllUsers(CancellationToken.None).ToListAsync();
			users.Should().BeEquivalentTo(expectedUsers, x => x.WithStrictOrdering().Excluding(y => y.Id));

			var roles = await userService.GetUserRoles(users.Single().Id, CancellationToken.None);
			roles.Should().BeEquivalentTo(expectedRoles, x => x.WithStrictOrdering().Excluding(y => y.Id));
		}

		[TestMethod]
		public async Task Initialize_SomeAdministratorExists_DoesNotCreateAnyUsers()
		{
			// Arrange

			await using var webApplicationFactory = new CustomWebApplicationFactory(authenticatedUser: null);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Identity/Account/Login"), CancellationToken.None);

			// Assert

			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var expectedUsers = new[]
			{
				SharedSeedData.PrivilegedUserName,
				SharedSeedData.LimitedUserName,
			};

			using var scopeServiceProvider = webApplicationFactory.Services.CreateScope();
			var userService = scopeServiceProvider.ServiceProvider.GetRequiredService<IUserService>();

			var users = (await userService.GetAllUsers(CancellationToken.None).ToListAsync()).Select(x => x.UserName);
			users.Should().BeEquivalentTo(expectedUsers, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Access_ForDefaultAdministrator_IsAllowed()
		{
			// Arrange

			await using var webApplicationFactory = new CustomWebApplicationFactory(authenticatedUser: ApplicationUser.DefaultAdministrator, seedData: new EmptySeedData());
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}
	}
}
