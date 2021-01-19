using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.IntegrationTests.Internal;
using MovieLibrary.UserManagement.Interfaces;

namespace MovieLibrary.IntegrationTests.Controllers
{
	[TestClass]
	public class UsersControllerTests
	{
		[TestMethod]
		public async Task Index_ForPrivilegedUser_ReturnsCorrectPage()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task Index_ForLimitedUser_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.LimitedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers"));
		}

		[TestMethod]
		public async Task GetRegisterUser_ForPrivilegedUser_ReturnsCorrectPage()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/RegisterUser"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task GetRegisterUser_ForLimitedUser_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.LimitedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/RegisterUser"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FRegisterUser"));
		}

		[TestMethod]
		public async Task PostRegisterUser_ForPrivilegedUser_RegistersUserCorrectly()
		{
			// Arrange

			using var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Email", "SomeNewUser@test.com"),
				new KeyValuePair<string, string>("Password", "Qwerty123!"),
				new KeyValuePair<string, string>("ConfirmPassword", "Qwerty123!"),
			});

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/RegisterUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/Users", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task PostRegisterUser_ForInvalidModel_ReturnsCorrectPage()
		{
			// Arrange

			using var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Email", String.Empty),
				new KeyValuePair<string, string>("Password", "Some Password"),
				new KeyValuePair<string, string>("ConfirmPassword", "Another Password"),
			});

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/RegisterUser"), formContent, CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task PostRegisterUser_ForLimitedUser_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Email", "SomeNewUser@test.com"),
				new KeyValuePair<string, string>("Password", "Qwerty123!"),
				new KeyValuePair<string, string>("ConfirmPassword", "Qwerty123!"),
			});

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.LimitedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/RegisterUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FRegisterUser"));
		}

		[TestMethod]
		public async Task EditUser_OfLastAdministratorUnderPrivilegedUser_ReturnsCorrectPage()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/EditUser/5eb7eb9e1fdada19f4eb59b0"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task EditUser_OfNonAdministratorUnderPrivilegedUser_ReturnsCorrectPage()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/EditUser/5eb7eb9f1fdada19f4eb59b1"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task EditUser_ForLimitedUser_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.LimitedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/EditUser/5eb7eb9f1fdada19f4eb59b1"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FEditUser%2F5eb7eb9f1fdada19f4eb59b1"));
		}

		[TestMethod]
		public async Task UpdateUser_ForPrivilegedUser_UpdatesUserCorrectly()
		{
			// Arrange

			using var formContent = new FormUrlEncodedContent(new[]
			{
				// Here we delete role "Limited User" and add role "Privileged User".
				new KeyValuePair<string, string>("Roles[0].RoleName", "Administrator"),
				new KeyValuePair<string, string>("Roles[1].Assigned", "true"),
				new KeyValuePair<string, string>("Roles[1].RoleName", "Privileged User"),
				new KeyValuePair<string, string>("Roles[2].RoleName", "Limited User"),
				new KeyValuePair<string, string>("UserId", "5eb7eb9f1fdada19f4eb59b1"),
				new KeyValuePair<string, string>("Roles[0].Assigned", "false"),
				new KeyValuePair<string, string>("Roles[1].Assigned", "false"),
				new KeyValuePair<string, string>("Roles[2].Assigned", "false"),
			});

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/UpdateUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/Users", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);

			using var scopeServiceProvider = webApplicationFactory.Services.CreateScope();
			var userService = scopeServiceProvider.ServiceProvider.GetRequiredService<IUserService>();
			var newUserRoles = await userService.GetUserRoles("5eb7eb9f1fdada19f4eb59b1", CancellationToken.None);

			CollectionAssert.AreEqual(new[] { "Privileged User", }, newUserRoles.Select(r => r.RoleName).ToList());
		}

		[TestMethod]
		public async Task UpdateUser_ForLimitedUser_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Roles[0].RoleName", "Administrator"),
				new KeyValuePair<string, string>("Roles[1].Assigned", "true"),
				new KeyValuePair<string, string>("Roles[1].RoleName", "Privileged User"),
				new KeyValuePair<string, string>("Roles[2].RoleName", "Limited User"),
				new KeyValuePair<string, string>("UserId", "5eb7eb9f1fdada19f4eb59b1"),
				new KeyValuePair<string, string>("Roles[0].Assigned", "false"),
				new KeyValuePair<string, string>("Roles[1].Assigned", "false"),
				new KeyValuePair<string, string>("Roles[2].Assigned", "false"),
			});

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.LimitedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/UpdateUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FUpdateUser"));
		}

		[TestMethod]
		public async Task ConfirmUserDeletion_ForPrivilegedUser_ReturnsCorrectPage()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/ConfirmUserDeletion/5eb7eb9f1fdada19f4eb59b1"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmUserDeletion_ForLimitedUser_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.LimitedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Users/ConfirmUserDeletion/5eb7eb9f1fdada19f4eb59b1"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FConfirmUserDeletion%2F5eb7eb9f1fdada19f4eb59b1"));
		}

		[TestMethod]
		public async Task DeleteUser_ForPrivilegedUser_ReturnsCorrectPage()
		{
			// Arrange

			using var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5eb7eb9f1fdada19f4eb59b1"),
			});

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/DeleteUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/Users", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/Users"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task DeleteUser_ForLimitedUser_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5eb7eb9f1fdada19f4eb59b1"),
			});

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.LimitedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Users/DeleteUser"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FUsers%2FDeleteUser"));
		}
	}
}
