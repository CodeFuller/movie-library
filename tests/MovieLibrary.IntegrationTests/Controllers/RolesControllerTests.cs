﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.IntegrationTests.Internal;
using MovieLibrary.UserManagement.Interfaces;
using static MovieLibrary.IntegrationTests.Internal.CustomWebApplicationFactory;

namespace MovieLibrary.IntegrationTests.Controllers
{
	[TestClass]
	public class RolesControllerTests
	{
		[TestMethod]
		public async Task Index_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(ApplicationUser.PrivilegedUser);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Roles"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task Index_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(ApplicationUser.LimitedUser);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Roles"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FRoles"));
		}

		[TestMethod]
		public async Task GetCreateRole_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(ApplicationUser.PrivilegedUser);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Roles/CreateRole"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task GetCreateRole_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(ApplicationUser.LimitedUser);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Roles/CreateRole"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FRoles%2FCreateRole"));
		}

		[TestMethod]
		public async Task PostCreateRole_ForAdministratorAccount_CreatesRoleCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Name", "Some New Role"),
				new KeyValuePair<string, string>("Permissions[0].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[0].PermissionName", "Permissions.MoviesToGet.Add"),
				new KeyValuePair<string, string>("Permissions[1].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[1].PermissionName", "Permissions.MoviesToGet.Read"),
				new KeyValuePair<string, string>("Permissions[2].PermissionName", "Permissions.MoviesToGet.MoveToMoviesToSee"),
				new KeyValuePair<string, string>("Permissions[3].PermissionName", "Permissions.MoviesToGet.Delete"),
				new KeyValuePair<string, string>("Permissions[4].PermissionName", "Permissions.MoviesToSee.Add"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[5].PermissionName", "Permissions.MoviesToSee.Read"),
				new KeyValuePair<string, string>("Permissions[6].PermissionName", "Permissions.MoviesToSee.MarkAsSeen"),
				new KeyValuePair<string, string>("Permissions[7].PermissionName", "Permissions.MoviesToSee.Delete"),
				new KeyValuePair<string, string>("Permissions[0].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[1].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[2].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[3].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[4].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[6].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[7].Assigned", "false"),
			});

			using var client = CreateHttpClient(ApplicationUser.PrivilegedUser);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Roles/CreateRole"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/Roles", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/Roles"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task PostCreateRole_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Name", "Some New Role"),
				new KeyValuePair<string, string>("Permissions[0].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[0].PermissionName", "Permissions.MoviesToGet.Add"),
				new KeyValuePair<string, string>("Permissions[1].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[1].PermissionName", "Permissions.MoviesToGet.Read"),
				new KeyValuePair<string, string>("Permissions[2].PermissionName", "Permissions.MoviesToGet.MoveToMoviesToSee"),
				new KeyValuePair<string, string>("Permissions[3].PermissionName", "Permissions.MoviesToGet.Delete"),
				new KeyValuePair<string, string>("Permissions[4].PermissionName", "Permissions.MoviesToSee.Add"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[5].PermissionName", "Permissions.MoviesToSee.Read"),
				new KeyValuePair<string, string>("Permissions[6].PermissionName", "Permissions.MoviesToSee.MarkAsSeen"),
				new KeyValuePair<string, string>("Permissions[7].PermissionName", "Permissions.MoviesToSee.Delete"),
				new KeyValuePair<string, string>("Permissions[0].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[1].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[2].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[3].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[4].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[6].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[7].Assigned", "false"),
			});

			using var client = CreateHttpClient(ApplicationUser.LimitedUser);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Roles/CreateRole"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FRoles%2FCreateRole"));
		}

		[TestMethod]
		public async Task EditRole_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(ApplicationUser.PrivilegedUser);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Roles/EditRole/5eb995ef4083c272a80ca308"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task EditRole_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(ApplicationUser.LimitedUser);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Roles/EditRole/5eb995ef4083c272a80ca308"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FRoles%2FEditRole%2F5eb995ef4083c272a80ca308"));
		}

		[TestMethod]
		public async Task UpdateRole_ForAdministratorAccount_UpdatesRoleCorrectly()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				// Here we delete permission "Permissions.MoviesToGet.Add" and add permission "Permissions.MoviesToSee.Add".
				new KeyValuePair<string, string>("Permissions[0].PermissionName", "Permissions.MoviesToGet.Add"),
				new KeyValuePair<string, string>("Permissions[1].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[1].PermissionName", "Permissions.MoviesToGet.Read"),
				new KeyValuePair<string, string>("Permissions[2].PermissionName", "Permissions.MoviesToGet.MoveToMoviesToSee"),
				new KeyValuePair<string, string>("Permissions[3].PermissionName", "Permissions.MoviesToGet.Delete"),
				new KeyValuePair<string, string>("Permissions[4].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[4].PermissionName", "Permissions.MoviesToSee.Add"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[5].PermissionName", "Permissions.MoviesToSee.Read"),
				new KeyValuePair<string, string>("Permissions[6].PermissionName", "Permissions.MoviesToSee.MarkAsSeen"),
				new KeyValuePair<string, string>("Permissions[7].PermissionName", "Permissions.MoviesToSee.Delete"),
				new KeyValuePair<string, string>("RoleId", "5eb995ef4083c272a80ca308"),
				new KeyValuePair<string, string>("Permissions[0].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[1].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[2].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[3].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[4].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[6].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[7].Assigned", "false"),
			});

			using var webApplicationFactory = new CustomWebApplicationFactory(ApplicationUser.PrivilegedUser);
			using var client = webApplicationFactory.CreateDefaultHttpClient();

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Roles/UpdateRole"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/Roles", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/Roles"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);

			using var scopeServiceProvider = webApplicationFactory.Services.CreateScope();
			var roleService = scopeServiceProvider.ServiceProvider.GetRequiredService<IRoleService>();
			var newPermissions = (await roleService.GetRolePermissions("5eb995ef4083c272a80ca308", CancellationToken.None))
				.OrderBy(r => r).ToList();

			CollectionAssert.AreEqual(new[] { "Permissions.MoviesToGet.Read", "Permissions.MoviesToSee.Add", "Permissions.MoviesToSee.Read" }, newPermissions.ToList());
		}

		[TestMethod]
		public async Task UpdateRole_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("Permissions[0].PermissionName", "Permissions.MoviesToGet.Add"),
				new KeyValuePair<string, string>("Permissions[1].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[1].PermissionName", "Permissions.MoviesToGet.Read"),
				new KeyValuePair<string, string>("Permissions[2].PermissionName", "Permissions.MoviesToGet.MoveToMoviesToSee"),
				new KeyValuePair<string, string>("Permissions[3].PermissionName", "Permissions.MoviesToGet.Delete"),
				new KeyValuePair<string, string>("Permissions[4].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[4].PermissionName", "Permissions.MoviesToSee.Add"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "true"),
				new KeyValuePair<string, string>("Permissions[5].PermissionName", "Permissions.MoviesToSee.Read"),
				new KeyValuePair<string, string>("Permissions[6].PermissionName", "Permissions.MoviesToSee.MarkAsSeen"),
				new KeyValuePair<string, string>("Permissions[7].PermissionName", "Permissions.MoviesToSee.Delete"),
				new KeyValuePair<string, string>("RoleId", "5eb995ef4083c272a80ca308"),
				new KeyValuePair<string, string>("Permissions[0].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[1].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[2].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[3].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[4].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[5].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[6].Assigned", "false"),
				new KeyValuePair<string, string>("Permissions[7].Assigned", "false"),
			});

			using var client = CreateHttpClient(ApplicationUser.LimitedUser);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Roles/UpdateRole"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FRoles%2FUpdateRole"));
		}

		[TestMethod]
		public async Task ConfirmRoleDeletion_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			using var client = CreateHttpClient(ApplicationUser.PrivilegedUser);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Roles/ConfirmRoleDeletion/5eb995ef4083c272a80ca308"), CancellationToken.None);

			// Assert

			await ResponseAssert.VerifyPageLoaded(response);
		}

		[TestMethod]
		public async Task ConfirmRoleDeletion_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			using var client = CreateHttpClient(ApplicationUser.LimitedUser);

			// Act

			using var response = await client.GetAsync(new Uri("https://localhost:5001/Roles/ConfirmRoleDeletion/5eb995ef4083c272a80ca308"), CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FRoles%2FConfirmRoleDeletion%2F5eb995ef4083c272a80ca308"));
		}

		[TestMethod]
		public async Task DeleteRole_ForAdministratorAccount_ReturnsCorrectPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5eb995ef4083c272a80ca308"),
			});

			using var client = CreateHttpClient(ApplicationUser.PrivilegedUser);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Roles/DeleteRole"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("/Roles", UriKind.Relative));

			using var indexResponse = await client.GetAsync(new Uri("https://localhost:5001/Roles"), CancellationToken.None);
			await ResponseAssert.VerifyPageLoaded(indexResponse);
		}

		[TestMethod]
		public async Task DeleteRole_ForUserAccount_RedirectsToAccessDeniedPage()
		{
			// Arrange

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("id", "5eb995ef4083c272a80ca308"),
			});

			using var client = CreateHttpClient(ApplicationUser.LimitedUser);

			// Act

			using var response = await client.PostAsync(new Uri("https://localhost:5001/Roles/DeleteRole"), formContent, CancellationToken.None);

			// Assert

			ResponseAssert.VerifyRedirect(response, new Uri("https://localhost:5001/Identity/Account/AccessDenied?ReturnUrl=%2FRoles%2FDeleteRole"));
		}
	}
}