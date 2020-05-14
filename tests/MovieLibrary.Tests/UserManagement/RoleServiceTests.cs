using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
using Moq.AutoMock;
using MovieLibrary.Authorization;
using MovieLibrary.Exceptions;
using MovieLibrary.UserManagement;
using MovieLibrary.UserManagement.Interfaces;

namespace MovieLibrary.Tests.UserManagement
{
	[TestClass]
	public class RoleServiceTests
	{
		[TestMethod]
		public async Task AssignRolePermissions_SomePermissionsAdded_AddsPermissionsCorrectly()
		{
			// Arrange

			var oldPermissions = new[]
			{
				"Permission #1",
				"Permission #3",
			};

			var newPermissions = new[]
			{
				"Permission #1",
				"Permission #2",
				"Permission #3",
				"Permission #4",
			};

			var role = new MongoRole();

			var mocker = new AutoMocker();

			var roleManagerMock = new Mock<IRoleManager<MongoRole, ObjectId>>();
			roleManagerMock.Setup(x => x.FindByIdAsync("SomeRoleId")).ReturnsAsync(role);
			roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(ToClaims(oldPermissions));
			roleManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<MongoRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(roleManagerMock);

			var target = mocker.CreateInstance<RoleService<MongoRole, ObjectId>>();

			// Act

			await target.AssignRolePermissions("SomeRoleId", newPermissions, CancellationToken.None);

			// Assert

			roleManagerMock.Verify(x => x.AddClaimAsync(role, It.IsAny<Claim>()), Times.Exactly(2));
			roleManagerMock.Verify(x => x.AddClaimAsync(role, It.Is<Claim>(c => IsPermissionClaim(c, "Permission #2"))), Times.Once);
			roleManagerMock.Verify(x => x.AddClaimAsync(role, It.Is<Claim>(c => IsPermissionClaim(c, "Permission #4"))), Times.Once);
		}

		[TestMethod]
		public async Task AssignRolePermissions_AddingOfPermissionFails_ThrowsUserManagementException()
		{
			// Arrange

			var oldPermissions = new[]
			{
				"Permission #1",
				"Permission #3",
			};

			var newPermissions = new[]
			{
				"Permission #1",
				"Permission #2",
				"Permission #3",
				"Permission #4",
			};

			var role = new MongoRole();

			var mocker = new AutoMocker();

			var roleManagerMock = new Mock<IRoleManager<MongoRole, ObjectId>>();
			roleManagerMock.Setup(x => x.FindByIdAsync("SomeRoleId")).ReturnsAsync(role);
			roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(ToClaims(oldPermissions));
			roleManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<MongoRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Failed());
			mocker.Use(roleManagerMock);

			var target = mocker.CreateInstance<RoleService<MongoRole, ObjectId>>();

			// Act

			Task Call() => target.AssignRolePermissions("SomeRoleId", newPermissions, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<UserManagementException>(Call);
		}

		[TestMethod]
		public async Task AssignRolePermissions_NoPermissionsAdded_DoesNotAddAnyPermissions()
		{
			// Arrange

			var oldPermissions = new[]
			{
				"Permission #1",
				"Permission #3",
			};

			var newPermissions = new[]
			{
				"Permission #1",
				"Permission #3",
			};

			var role = new MongoRole();

			var mocker = new AutoMocker();

			var roleManagerMock = new Mock<IRoleManager<MongoRole, ObjectId>>();
			roleManagerMock.Setup(x => x.FindByIdAsync("SomeRoleId")).ReturnsAsync(role);
			roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(ToClaims(oldPermissions));
			mocker.Use(roleManagerMock);

			var target = mocker.CreateInstance<RoleService<MongoRole, ObjectId>>();

			// Act

			await target.AssignRolePermissions("SomeRoleId", newPermissions, CancellationToken.None);

			// Assert

			roleManagerMock.Verify(x => x.AddClaimAsync(It.IsAny<MongoRole>(), It.IsAny<Claim>()), Times.Never);
		}

		[TestMethod]
		public async Task AssignRolePermissions_SomePermissionsRemoved_RemovesPermissionsCorrectly()
		{
			// Arrange

			var oldPermissions = new[]
			{
				"Permission #1",
				"Permission #2",
				"Permission #3",
				"Permission #4",
			};

			var newPermissions = new[]
			{
				"Permission #1",
				"Permission #3",
			};

			var role = new MongoRole();

			var mocker = new AutoMocker();

			var roleManagerMock = new Mock<IRoleManager<MongoRole, ObjectId>>();
			roleManagerMock.Setup(x => x.FindByIdAsync("SomeRoleId")).ReturnsAsync(role);
			roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(ToClaims(oldPermissions));
			roleManagerMock.Setup(x => x.RemoveClaimAsync(It.IsAny<MongoRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(roleManagerMock);

			var target = mocker.CreateInstance<RoleService<MongoRole, ObjectId>>();

			// Act

			await target.AssignRolePermissions("SomeRoleId", newPermissions, CancellationToken.None);

			// Assert

			roleManagerMock.Verify(x => x.RemoveClaimAsync(role, It.IsAny<Claim>()), Times.Exactly(2));
			roleManagerMock.Verify(x => x.RemoveClaimAsync(role, It.Is<Claim>(c => IsPermissionClaim(c, "Permission #2"))), Times.Once);
			roleManagerMock.Verify(x => x.RemoveClaimAsync(role, It.Is<Claim>(c => IsPermissionClaim(c, "Permission #4"))), Times.Once);
		}

		[TestMethod]
		public async Task AssignRolePermissions_RemovingOfPermissionFails_ThrowsUserManagementException()
		{
			// Arrange

			var oldPermissions = new[]
			{
				"Permission #1",
				"Permission #2",
				"Permission #3",
				"Permission #4",
			};

			var newPermissions = new[]
			{
				"Permission #1",
				"Permission #3",
			};

			var role = new MongoRole();

			var mocker = new AutoMocker();

			var roleManagerMock = new Mock<IRoleManager<MongoRole, ObjectId>>();
			roleManagerMock.Setup(x => x.FindByIdAsync("SomeRoleId")).ReturnsAsync(role);
			roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(ToClaims(oldPermissions));
			roleManagerMock.Setup(x => x.RemoveClaimAsync(It.IsAny<MongoRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Failed());
			mocker.Use(roleManagerMock);

			var target = mocker.CreateInstance<RoleService<MongoRole, ObjectId>>();

			// Act

			Task Call() => target.AssignRolePermissions("SomeRoleId", newPermissions, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<UserManagementException>(Call);
		}

		[TestMethod]
		public async Task AssignRolePermissions_NoPermissionsRemoved_DoesNotRemoveAnyPermissions()
		{
			// Arrange

			var oldPermissions = new[]
			{
				"Permission #1",
				"Permission #3",
			};

			var newPermissions = new[]
			{
				"Permission #1",
				"Permission #3",
			};

			var role = new MongoRole();

			var mocker = new AutoMocker();

			var roleManagerMock = new Mock<IRoleManager<MongoRole, ObjectId>>();
			roleManagerMock.Setup(x => x.FindByIdAsync("SomeRoleId")).ReturnsAsync(role);
			roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(ToClaims(oldPermissions));
			mocker.Use(roleManagerMock);

			var target = mocker.CreateInstance<RoleService<MongoRole, ObjectId>>();

			// Act

			await target.AssignRolePermissions("SomeRoleId", newPermissions, CancellationToken.None);

			// Assert

			roleManagerMock.Verify(x => x.RemoveClaimAsync(It.IsAny<MongoRole>(), It.IsAny<Claim>()), Times.Never);
		}

		[TestMethod]
		public async Task AssignRolePermissions_ForBuiltInRole_ThrowsUserManagementException()
		{
			// Arrange

			var oldPermissions = new[]
			{
				"Permission #1",
				"Permission #2",
			};

			var newPermissions = new[]
			{
				"Permission #1",
				"Permission #2",
			};

			var role = new MongoRole(SecurityConstants.AdministratorRole);

			var mocker = new AutoMocker();

			var roleManagerMock = new Mock<IRoleManager<MongoRole, ObjectId>>();
			roleManagerMock.Setup(x => x.FindByIdAsync("SomeRoleId")).ReturnsAsync(role);
			roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(ToClaims(oldPermissions));
			roleManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<MongoRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(roleManagerMock);

			var target = mocker.CreateInstance<RoleService<MongoRole, ObjectId>>();

			// Act

			Task Call() => target.AssignRolePermissions("SomeRoleId", newPermissions, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<UserManagementException>(Call);
		}

		[TestMethod]
		public async Task DeleteRole_ForBuiltInRole_ThrowsUserManagementException()
		{
			// Arrange

			var role = new MongoRole(SecurityConstants.AdministratorRole);

			var mocker = new AutoMocker();

			var roleManagerMock = new Mock<IRoleManager<MongoRole, ObjectId>>();
			roleManagerMock.Setup(x => x.FindByIdAsync("SomeRoleId")).ReturnsAsync(role);
			roleManagerMock.Setup(x => x.DeleteAsync(It.IsAny<MongoRole>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(roleManagerMock);

			var target = mocker.CreateInstance<RoleService<MongoRole, ObjectId>>();

			// Act

			Task Call() => target.DeleteRole("SomeRoleId", CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<UserManagementException>(Call);
		}

		private static IList<Claim> ToClaims(IEnumerable<string> permissions)
		{
			return permissions.Select(p => new Claim(SecurityConstants.PermissionClaimType, p))
				.ToList();
		}

		private static bool IsPermissionClaim(Claim claim, string permission)
		{
			return claim.Type == SecurityConstants.PermissionClaimType && claim.Value == permission;
		}
	}
}
