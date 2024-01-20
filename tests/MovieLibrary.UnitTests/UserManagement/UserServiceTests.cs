using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using MovieLibrary.Authorization;
using MovieLibrary.Exceptions;
using MovieLibrary.UserManagement;
using MovieLibrary.UserManagement.Interfaces;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UnitTests.UserManagement
{
	[TestClass]
	public class UserServiceTests
	{
		[TestMethod]
		public async Task GetAllUsers_ForDefaultAdministrator_SetsCanBeEditedToFalse()
		{
			// Arrange

			var defaultAdmin = new IdentityUser
			{
				Id = "Id1",
				UserName = SecurityConstants.DefaultAdministratorEmail,
			};

			var customAdmin = new IdentityUser
			{
				Id = "Id2",
				UserName = "Custom Admin",
			};

			var limitedUser = new IdentityUser
			{
				Id = "Id3",
				UserName = "Limited User",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.Users).Returns(new[] { defaultAdmin, customAdmin, limitedUser }.AsQueryable());
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { defaultAdmin, customAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var users = await target.GetAllUsers(CancellationToken.None).ToListAsync();

			// Assert

			var expectedUser = new UserModel
			{
				Id = "Id1",
				UserName = SecurityConstants.DefaultAdministratorEmail,
				CanBeEdited = false,
			};

			users[0].Should().BeEquivalentTo(expectedUser, x => x.Excluding(y => y.CanBeDeleted));
		}

		[TestMethod]
		public async Task GetAllUsers_ForCustomAdministrator_SetsCanBeEditedToTrue()
		{
			// Arrange

			var customAdmin = new IdentityUser
			{
				Id = "Id1",
				UserName = "Custom Admin",
			};

			var limitedUser = new IdentityUser
			{
				Id = "Id2",
				UserName = "Limited User",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.Users).Returns(new[] { customAdmin, limitedUser }.AsQueryable());
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { customAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var users = await target.GetAllUsers(CancellationToken.None).ToListAsync();

			// Assert

			var expectedUser = new UserModel
			{
				Id = "Id1",
				UserName = "Custom Admin",
				CanBeEdited = true,
			};

			users[0].Should().BeEquivalentTo(expectedUser, x => x.Excluding(y => y.CanBeDeleted));
		}

		[TestMethod]
		public async Task GetAllUsers_SingleDefaultAdmin_SetsCanBeDeletedToFalse()
		{
			// Arrange

			var singleAdmin = new IdentityUser
			{
				Id = "Id1",
				UserName = SecurityConstants.DefaultAdministratorEmail,
			};

			var limitedUser = new IdentityUser
			{
				Id = "Id2",
				UserName = "Limited User",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.Users).Returns(new[] { singleAdmin, limitedUser }.AsQueryable());
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { singleAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var users = await target.GetAllUsers(CancellationToken.None).ToListAsync();

			// Assert

			var expectedUsers = new[]
			{
				new UserModel
				{
					Id = "Id1",
					UserName = SecurityConstants.DefaultAdministratorEmail,
					CanBeDeleted = false,
				},
				new UserModel
				{
					Id = "Id2",
					UserName = "Limited User",
					CanBeDeleted = true,
				},
			};

			users.Should().BeEquivalentTo(expectedUsers, x => x.WithStrictOrdering().Excluding(y => y.CanBeEdited));
		}

		[TestMethod]
		public async Task GetAllUsers_SingleCustomAdmin_SetsCanBeDeletedToFalse()
		{
			// Arrange

			var singleAdmin = new IdentityUser
			{
				Id = "Id1",
				UserName = "Custom Admin",
			};

			var limitedUser = new IdentityUser
			{
				Id = "Id2",
				UserName = "Limited User",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.Users).Returns(new[] { singleAdmin, limitedUser }.AsQueryable());
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { singleAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var users = await target.GetAllUsers(CancellationToken.None).ToListAsync();

			// Assert

			var expectedUsers = new[]
			{
				new UserModel
				{
					Id = "Id1",
					UserName = "Custom Admin",
					CanBeDeleted = false,
				},
				new UserModel
				{
					Id = "Id2",
					UserName = "Limited User",
					CanBeDeleted = true,
				},
			};

			users.Should().BeEquivalentTo(expectedUsers, x => x.WithStrictOrdering().Excluding(y => y.CanBeEdited));
		}

		[TestMethod]
		public async Task GetAllUsers_MultipleAdmins_SetsCanBeDeletedToTrue()
		{
			// Arrange

			var defaultAdmin = new IdentityUser
			{
				Id = "Id1",
				UserName = SecurityConstants.DefaultAdministratorEmail,
			};

			var customAdmin = new IdentityUser
			{
				Id = "Id2",
				UserName = "Custom Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.Users).Returns(new[] { defaultAdmin, customAdmin }.AsQueryable());
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { defaultAdmin, customAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var users = await target.GetAllUsers(CancellationToken.None).ToListAsync();

			// Assert

			var expectedUsers = new[]
			{
				new UserModel
				{
					Id = "Id1",
					UserName = SecurityConstants.DefaultAdministratorEmail,
					CanBeDeleted = true,
				},
				new UserModel
				{
					Id = "Id2",
					UserName = "Custom Admin",
					CanBeDeleted = true,
				},
			};

			users.Should().BeEquivalentTo(expectedUsers, x => x.WithStrictOrdering().Excluding(y => y.CanBeEdited));
		}

		[TestMethod]
		public async Task GetUserRoles_SingleDefaultAdmin_SetsReadOnlyToTrue()
		{
			// Arrange

			var singleAdmin = new IdentityUser
			{
				Id = "SomeId",
				UserName = SecurityConstants.DefaultAdministratorEmail,
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeId")).ReturnsAsync(singleAdmin);
			userManagerMock.Setup(x => x.GetRolesAsync(singleAdmin)).ReturnsAsync(new[] { SecurityConstants.AdministratorRole, "Another Role" });
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { singleAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var roles = (await target.GetUserRoles("SomeId", CancellationToken.None)).ToList();

			// Assert

			var expectedRoles = new[]
			{
				new UserRoleModel
				{
					RoleName = SecurityConstants.AdministratorRole,
					ReadOnly = true,
				},

				new UserRoleModel
				{
					RoleName = "Another Role",
					ReadOnly = false,
				},
			};

			roles.Should().BeEquivalentTo(expectedRoles, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task GetUserRoles_SingleCustomAdmin_SetsReadOnlyToTrue()
		{
			// Arrange

			var singleAdmin = new IdentityUser
			{
				Id = "SomeId",
				UserName = "Custom Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeId")).ReturnsAsync(singleAdmin);
			userManagerMock.Setup(x => x.GetRolesAsync(singleAdmin)).ReturnsAsync(new[] { SecurityConstants.AdministratorRole, "Another Role" });
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { singleAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var roles = (await target.GetUserRoles("SomeId", CancellationToken.None)).ToList();

			// Assert

			var expectedRoles = new[]
			{
				new UserRoleModel
				{
					RoleName = SecurityConstants.AdministratorRole,
					ReadOnly = true,
				},

				new UserRoleModel
				{
					RoleName = "Another Role",
					ReadOnly = false,
				},
			};

			roles.Should().BeEquivalentTo(expectedRoles, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task GetUserRoles_MultipleAdmins_SetsReadOnlyToFalse()
		{
			// Arrange

			var defaultAdmin = new IdentityUser
			{
				Id = "Id1",
				UserName = SecurityConstants.DefaultAdministratorEmail,
			};

			var customAdmin = new IdentityUser
			{
				Id = "Id2",
				UserName = "Custom Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("Id2")).ReturnsAsync(customAdmin);
			userManagerMock.Setup(x => x.GetRolesAsync(customAdmin)).ReturnsAsync(new[] { SecurityConstants.AdministratorRole, "Another Role" });
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { defaultAdmin, customAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var roles = (await target.GetUserRoles("Id2", CancellationToken.None)).ToList();

			// Assert

			var expectedRoles = new[]
			{
				new UserRoleModel
				{
					RoleName = SecurityConstants.AdministratorRole,
					ReadOnly = false,
				},

				new UserRoleModel
				{
					RoleName = "Another Role",
					ReadOnly = false,
				},
			};

			roles.Should().BeEquivalentTo(expectedRoles, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task GetUserRoles_ForNonAdministrator_SetsReadOnlyToFalse()
		{
			// Arrange

			var customAdmin = new IdentityUser
			{
				Id = "Id1",
				UserName = "Custom Admin",
			};

			var user = new IdentityUser
			{
				Id = "Id2",
				UserName = "Limited User",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("Id2")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new[] { "Role #1", "Role #2" });
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { customAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var roles = (await target.GetUserRoles("Id2", CancellationToken.None)).ToList();

			// Assert

			var expectedRoles = new[]
			{
				new UserRoleModel
				{
					RoleName = "Role #1",
					ReadOnly = false,
				},

				new UserRoleModel
				{
					RoleName = "Role #2",
					ReadOnly = false,
				},
			};

			roles.Should().BeEquivalentTo(expectedRoles, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task AssignUserRoles_SomeRolesAdded_AddsRolesCorrectly()
		{
			// Arrange

			var oldRoles = new[]
			{
				"Role #1",
				"Role #3",
			};

			var newRoles = new[]
			{
				"Role #1",
				"Role #2",
				"Role #3",
				"Role #4",
			};

			var user = new IdentityUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			await target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			Func<IEnumerable<string>, bool> checkRoles = addedRoles =>
			{
				return new[] { "Role #2", "Role #4", }.SequenceEqual(addedRoles.OrderBy(r => r));
			};

			userManagerMock.Verify(x => x.AddToRolesAsync(user, It.Is<IEnumerable<string>>(r => checkRoles(r))), Times.Once);
		}

		[TestMethod]
		public async Task AssignUserRoles_AddingOfRolesFails_ThrowsUserManagementException()
		{
			// Arrange

			var oldRoles = new[]
			{
				"Role #1",
				"Role #3",
			};

			var newRoles = new[]
			{
				"Role #1",
				"Role #2",
				"Role #3",
				"Role #4",
			};

			var user = new IdentityUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Failed());
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var call = () => target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<UserManagementException>();
		}

		[TestMethod]
		public async Task AssignUserRoles_NoRolesAdded_DoesNotAddAnyRoles()
		{
			// Arrange

			var oldRoles = new[]
			{
				"Role #1",
				"Role #3",
			};

			var newRoles = new[]
			{
				"Role #1",
				"Role #3",
			};

			var user = new IdentityUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			await target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			userManagerMock.Verify(x => x.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
		}

		[TestMethod]
		public async Task AssignUserRoles_SomeRolesRemoved_RemovesRolesCorrectly()
		{
			// Arrange

			var oldRoles = new[]
			{
				"Role #1",
				"Role #2",
				"Role #3",
				"Role #4",
			};

			var newRoles = new[]
			{
				"Role #1",
				"Role #3",
			};

			var user = new IdentityUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.RemoveFromRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			await target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			Func<IEnumerable<string>, bool> checkRoles = removedRoles =>
			{
				return new[] { "Role #2", "Role #4", }.SequenceEqual(removedRoles.OrderBy(r => r));
			};

			userManagerMock.Verify(x => x.RemoveFromRolesAsync(user, It.Is<IEnumerable<string>>(r => checkRoles(r))), Times.Once);
		}

		[TestMethod]
		public async Task AssignUserRoles_RemovingOfRolesFails_ThrowsUserManagementException()
		{
			// Arrange

			var oldRoles = new[]
			{
				"Role #1",
				"Role #2",
				"Role #3",
				"Role #4",
			};

			var newRoles = new[]
			{
				"Role #1",
				"Role #3",
			};

			var user = new IdentityUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.RemoveFromRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Failed());
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var call = () => target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<UserManagementException>();
		}

		[TestMethod]
		public async Task AssignUserRoles_NoRolesRemoved_DoesNotRemoveAnyRoles()
		{
			// Arrange

			var oldRoles = new[]
			{
				"Role #1",
				"Role #3",
			};

			var newRoles = new[]
			{
				"Role #1",
				"Role #3",
			};

			var user = new IdentityUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			await target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			userManagerMock.Verify(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()), Times.Never);
		}

		[TestMethod]
		public async Task AssignUserRoles_ForDefaultAdministrator_ThrowsUserManagementException()
		{
			// Arrange

			var oldRoles = new[]
			{
				"Role #1",
				"Role #3",
			};

			var newRoles = new[]
			{
				"Role #1",
				"Role #3",
			};

			var user = new IdentityUser(SecurityConstants.DefaultAdministratorEmail);

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var call = () => target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<UserManagementException>();
		}

		[TestMethod]
		public async Task AssignUserRoles_AdministratorRoleRemovedFromLastAdministrator_ThrowsUserManagementException()
		{
			// Arrange

			var oldRoles = new[]
			{
				SecurityConstants.AdministratorRole,
				"Custom Role",
			};

			var newRoles = new[]
			{
				"Custom Role",
			};

			var admin = new IdentityUser("Custom Admin");

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(admin);
			userManagerMock.Setup(x => x.GetRolesAsync(admin)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { admin });
			userManagerMock.Setup(x => x.RemoveFromRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var call = () => target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<UserManagementException>();
		}

		[TestMethod]
		public async Task AssignUserRoles_NonAdministratorRoleRemovedFromLastAdministrator_RemovesRoleCorrectly()
		{
			// Arrange

			var oldRoles = new[]
			{
				SecurityConstants.AdministratorRole,
				"Custom Role",
			};

			var newRoles = new[]
			{
				SecurityConstants.AdministratorRole,
			};

			var admin = new IdentityUser
			{
				Id = "SomeUserId",
				UserName = "Custom Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(admin);
			userManagerMock.Setup(x => x.GetRolesAsync(admin)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { admin });
			userManagerMock.Setup(x => x.RemoveFromRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			await target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			userManagerMock.Verify(x => x.RemoveFromRolesAsync(admin, new[] { "Custom Role" }), Times.Once);
		}

		[TestMethod]
		public async Task AssignUserRoles_AdministratorRoleRemovedFromNonLastAdministrator_RemovesRoleCorrectly()
		{
			// Arrange

			var oldRoles = new[]
			{
				SecurityConstants.AdministratorRole,
				"Custom Role",
			};

			var newRoles = new[]
			{
				"Custom Role",
			};

			var admin = new IdentityUser
			{
				Id = "Id1",
				UserName = "Custom Admin",
			};

			var anotherAdmin = new IdentityUser
			{
				Id = "Id2",
				UserName = "Another Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("Id1")).ReturnsAsync(admin);
			userManagerMock.Setup(x => x.GetRolesAsync(admin)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { admin, anotherAdmin });
			userManagerMock.Setup(x => x.RemoveFromRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			await target.AssignUserRoles("Id1", newRoles, CancellationToken.None);

			// Assert

			userManagerMock.Verify(x => x.RemoveFromRolesAsync(admin, new[] { SecurityConstants.AdministratorRole }), Times.Once);
		}

		[TestMethod]
		public async Task DeleteUser_ForLastAdministrator_ThrowsUserManagementException()
		{
			// Arrange

			var user = new IdentityUser
			{
				Id = "SomeId",
				UserName = "Some Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { user });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			var call = () => target.DeleteUser("SomeId", CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<UserManagementException>();
		}

		[TestMethod]
		public async Task DeleteUser_ForNotLastAdministrator_DeletesUsers()
		{
			// Arrange

			var admin1 = new IdentityUser
			{
				Id = "Id1",
				UserName = "Admin #1",
			};

			var admin2 = new IdentityUser
			{
				Id = "Id2",
				UserName = "Admin #2",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("Id1")).ReturnsAsync(admin1);
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { admin1, admin2 });
			userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<IdentityUser>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			await target.DeleteUser("Id1", CancellationToken.None);

			// Assert

			userManagerMock.Verify(x => x.DeleteAsync(admin1), Times.Once);
		}

		[TestMethod]
		public async Task DeleteUser_ForNonAdministrator_DeletesUsers()
		{
			// Arrange

			var user = new IdentityUser
			{
				Id = "Id1",
				UserName = "Some User",
			};

			var singleAdmin = new IdentityUser
			{
				Id = "Id2",
				UserName = "Single Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<IdentityUser, string>>();
			userManagerMock.Setup(x => x.FindByIdAsync("Id1")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { singleAdmin });
			userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<IdentityUser>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<IdentityUser, string>>();

			// Act

			await target.DeleteUser("Id1", CancellationToken.None);

			// Assert

			userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
		}
	}
}
