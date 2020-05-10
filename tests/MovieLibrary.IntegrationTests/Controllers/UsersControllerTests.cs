using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.IntegrationTests.Internal;
using MovieLibrary.UserManagement;
using static MovieLibrary.IntegrationTests.Internal.CustomWebApplicationFactory;

namespace MovieLibrary.IntegrationTests.Controllers
{
	[TestClass]
	public class UsersControllerTests
	{
		[TestMethod]
		public async Task Index_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task Index_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers"));
		}

		[TestMethod]
		public async Task GetRegisterUser_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/RegisterUser"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task GetRegisterUser_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/RegisterUser"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FRegisterUser"));
		}

		[TestMethod]
		public async Task PostRegisterUser_ForAdministratorAccount_RegistersUserCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Email", "SomeNewUser@test.com"),
				new KeyValuePair<string, string>("Password", "Qwerty123!"),
				new KeyValuePair<string, string>("ConfirmPassword", "Qwerty123!"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/RegisterUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/Users", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task PostRegisterUser_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Email", "SomeNewUser@test.com"),
				new KeyValuePair<string, string>("Password", "Qwerty123!"),
				new KeyValuePair<string, string>("ConfirmPassword", "Qwerty123!"),
			});

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/RegisterUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FRegisterUser"));
		}

		[TestMethod]
		public async Task EditUser_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/EditUser/5eb7eb9f1fdada19f4eb59b1"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task EditUser_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/EditUser/5eb7eb9f1fdada19f4eb59b1"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FEditUser%2F5eb7eb9f1fdada19f4eb59b1"));
		}

		[TestMethod]
		public async Task UpdateUser_ForAdministratorAccount_UpdatesUserCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				// Here we delete permission CanAddMoviesToGet and add permission CanAddMoviesToSee.
				new KeyValuePair<string, string>("Permissions[0].PermissionName", "Administrator"),
				new KeyValuePair<string, string>("Permissions[1].PermissionName", "CanAddMoviesToGet"),
				new KeyValuePair<string, string>("Permissions[2].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[2].PermissionName", "CanReadMoviesToGet"),
				new KeyValuePair<string, string>("Permissions[3].PermissionName", "CanDeleteMoviesToGet"),
				new KeyValuePair<string, string>("Permissions[4].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[4].PermissionName", "CanAddMoviesToSee"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[5].PermissionName", "CanReadMoviesToSee"),
				new KeyValuePair<string, string>("Permissions[6].PermissionName", "CanMarkMoviesAsSeen"),
				new KeyValuePair<string, string>("Permissions[7].PermissionName", "CanDeleteMoviesToSee"),
				new KeyValuePair<string, string>("UserId", "5eb7eb9f1fdada19f4eb59b1"),
				new KeyValuePair<string, string>("Permissions[0].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[1].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[2].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[3].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[4].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[6].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[7].Assigned", "false"),
			});

			var webApplicationFactory = new CustomWebApplicationFactory(UserRoles.AdministratorRoles);
			using var client = webApplicationFactory.CreateHttpClient();

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/UpdateUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/Users", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);

			using var scopeServiceProvider = webApplicationFactory.Services.CreateScope();
			var userService = scopeServiceProvider.ServiceProvider.GetRequiredService<IUserService>();
			var updatedUser = await userService.GetUser("5eb7eb9f1fdada19f4eb59b1", CancellationToken.None);

			CollectionAssert.AreEqual(new[] { "CanReadMoviesToGet", "CanReadMoviesToSee", "CanAddMoviesToSee", }, updatedUser.UserPermissions.ToList());
		}

		[TestMethod]
		public async Task UpdateUser_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Permissions[0].PermissionName", "Administrator"),
				new KeyValuePair<string, string>("Permissions[1].PermissionName", "CanAddMoviesToGet"),
				new KeyValuePair<string, string>("Permissions[2].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[2].PermissionName", "CanReadMoviesToGet"),
				new KeyValuePair<string, string>("Permissions[3].PermissionName", "CanDeleteMoviesToGet"),
				new KeyValuePair<string, string>("Permissions[4].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[4].PermissionName", "CanAddMoviesToSee"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[5].PermissionName", "CanReadMoviesToSee"),
				new KeyValuePair<string, string>("Permissions[6].PermissionName", "CanMarkMoviesAsSeen"),
				new KeyValuePair<string, string>("Permissions[7].PermissionName", "CanDeleteMoviesToSee"),
				new KeyValuePair<string, string>("UserId", "5eb7eb9f1fdada19f4eb59b1"),
				new KeyValuePair<string, string>("Permissions[0].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[1].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[2].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[3].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[4].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[6].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[7].Assigned", "false"),
			});

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/UpdateUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FUpdateUser"));
		}

		[TestMethod]
		public async Task ConfirmUserDeletion_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/ConfirmUserDeletion/5eb7eb9f1fdada19f4eb59b1"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmUserDeletion_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/ConfirmUserDeletion/5eb7eb9f1fdada19f4eb59b1"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FConfirmUserDeletion%2F5eb7eb9f1fdada19f4eb59b1"));
		}

		[TestMethod]
		public async Task DeleteUser_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5eb7eb9f1fdada19f4eb59b1"),
			});

			using var client = CreateHttpClient(UserRoles.AdministratorRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/DeleteUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/Users", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task DeleteUser_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5eb7eb9f1fdada19f4eb59b1"),
			});

			using var client = CreateHttpClient(UserRoles.LimitedUserRoles);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/DeleteUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FDeleteUser"));
		}
	}
}
