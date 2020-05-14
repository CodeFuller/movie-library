using System;
using System.Collections.Generic;
using System.Linq;
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
	public class UserServiceTests
	{
		[TestMethod]
		public async Task GetAllUsers_ForDefaultAdministrator_SetsCanBeEditedToTrue()
		{
			// Arrange

			var defaultAdmin = new MongoUser
			{
				Id = new ObjectId("5eb69e3e3233d47930edf61b"),
				UserName = SecurityConstants.DefaultAdministratorEmail,
			};

			var customAdmin = new MongoUser
			{
				Id = new ObjectId("5eb7022731bed923601da956"),
				UserName = "Custom Admin",
			};

			var limitedUser = new MongoUser
			{
				Id = new ObjectId("5eb705a69c615e5ca0f29a8b"),
				UserName = "Limited User",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.Users).Returns(new[] { defaultAdmin, customAdmin, limitedUser }.AsQueryable());
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { defaultAdmin, customAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			var users = await target.GetAllUsers(CancellationToken.None).ToListAsync();

			// Assert

			Assert.AreEqual(SecurityConstants.DefaultAdministratorEmail, users[0].UserName);
			Assert.IsFalse(users[0].CanBeEdited);
		}

		[TestMethod]
		public async Task GetAllUsers_ForCustomAdministrator_SetsCanBeEditedToTrue()
		{
			// Arrange

			var customAdmin = new MongoUser
			{
				Id = new ObjectId("5eb7022731bed923601da956"),
				UserName = "Custom Admin",
			};

			var limitedUser = new MongoUser
			{
				Id = new ObjectId("5eb705a69c615e5ca0f29a8b"),
				UserName = "Limited User",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.Users).Returns(new[] { customAdmin, limitedUser }.AsQueryable());
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { customAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			var users = await target.GetAllUsers(CancellationToken.None).ToListAsync();

			// Assert

			Assert.IsTrue(users[0].CanBeEdited);
		}

		[DataTestMethod]
		public async Task GetAllUsers_SingleDefaultAdmin_SetsCanBeDeletedToFalse()
		{
			// Arrange

			var singleAdmin = new MongoUser
			{
				Id = new ObjectId("5eb69e3e3233d47930edf61b"),
				UserName = SecurityConstants.DefaultAdministratorEmail,
			};

			var limitedUser = new MongoUser
			{
				Id = new ObjectId("5eb705a69c615e5ca0f29a8b"),
				UserName = "Limited User",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.Users).Returns(new[] { singleAdmin, limitedUser }.AsQueryable());
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { singleAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			var users = await target.GetAllUsers(CancellationToken.None).ToListAsync();

			// Assert

			Assert.AreEqual(SecurityConstants.DefaultAdministratorEmail, users[0].UserName);
			Assert.IsFalse(users[0].CanBeDeleted);

			Assert.AreEqual("Limited User", users[1].UserName);
			Assert.IsTrue(users[1].CanBeDeleted);
		}

		[DataTestMethod]
		public async Task GetAllUsers_SingleCustomAdmin_SetsCanBeDeletedToFalse()
		{
			// Arrange

			var singleAdmin = new MongoUser
			{
				Id = new ObjectId("5eb69e3e3233d47930edf61b"),
				UserName = "Custom Admin",
			};

			var limitedUser = new MongoUser
			{
				Id = new ObjectId("5eb705a69c615e5ca0f29a8b"),
				UserName = "Limited User",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.Users).Returns(new[] { singleAdmin, limitedUser }.AsQueryable());
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { singleAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			var users = await target.GetAllUsers(CancellationToken.None).ToListAsync();

			// Assert

			Assert.AreEqual("Custom Admin", users[0].UserName);
			Assert.IsFalse(users[0].CanBeDeleted);

			Assert.AreEqual("Limited User", users[1].UserName);
			Assert.IsTrue(users[1].CanBeDeleted);
		}

		[DataTestMethod]
		public async Task GetAllUsers_MultipleAdmins_SetsCanBeDeletedToTrue()
		{
			// Arrange

			var defaultAdmin = new MongoUser
			{
				Id = new ObjectId("5eb705a69c615e5ca0f29a8b"),
				UserName = SecurityConstants.DefaultAdministratorEmail,
			};

			var customAdmin = new MongoUser
			{
				Id = new ObjectId("5eb69e3e3233d47930edf61b"),
				UserName = "Custom Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.Users).Returns(new[] { defaultAdmin, customAdmin }.AsQueryable());
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { defaultAdmin, customAdmin });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			var users = await target.GetAllUsers(CancellationToken.None).ToListAsync();

			// Assert

			Assert.IsTrue(users[0].CanBeDeleted);
			Assert.IsTrue(users[1].CanBeDeleted);
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

			var user = new MongoUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.AddToRolesAsync(It.IsAny<MongoUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

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

			var user = new MongoUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.AddToRolesAsync(It.IsAny<MongoUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Failed());
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			Task Call() => target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<UserManagementException>(Call);
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

			var user = new MongoUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			await target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			userManagerMock.Verify(x => x.AddToRolesAsync(It.IsAny<MongoUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
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

			var user = new MongoUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.RemoveFromRolesAsync(It.IsAny<MongoUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

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

			var user = new MongoUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			userManagerMock.Setup(x => x.RemoveFromRolesAsync(It.IsAny<MongoUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Failed());
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			Task Call() => target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<UserManagementException>(Call);
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

			var user = new MongoUser();

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

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

			var user = new MongoUser(SecurityConstants.DefaultAdministratorEmail);

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.FindByIdAsync("SomeUserId")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(oldRoles);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			Task Call() => target.AssignUserRoles("SomeUserId", newRoles, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<UserManagementException>(Call);
		}

		[TestMethod]
		public async Task DeleteUser_ForLastAdministrator_ThrowsUserManagementException()
		{
			// Arrange

			var user = new MongoUser
			{
				Id = new ObjectId("5eb69e3e3233d47930edf61b"),
				UserName = "Some Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.FindByIdAsync("5eb69e3e3233d47930edf61b")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { user });
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			Task Call() => target.DeleteUser("5eb69e3e3233d47930edf61b",  CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<UserManagementException>(Call);
		}

		[TestMethod]
		public async Task DeleteUser_ForNotLastAdministrator_DeletesUsers()
		{
			// Arrange

			var user = new MongoUser
			{
				Id = new ObjectId("5eb69e3e3233d47930edf61b"),
				UserName = "Some Admin",
			};

			var anotherAdmin = new MongoUser
			{
				Id = new ObjectId("5eb7022731bed923601da956"),
				UserName = "Another Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.FindByIdAsync("5eb69e3e3233d47930edf61b")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { user, anotherAdmin });
			userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<MongoUser>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			await target.DeleteUser("5eb69e3e3233d47930edf61b", CancellationToken.None);

			// Assert

			userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
		}

		[TestMethod]
		public async Task DeleteUser_ForNonAdministrator_DeletesUsers()
		{
			// Arrange

			var user = new MongoUser
			{
				Id = new ObjectId("5eb69e3e3233d47930edf61b"),
				UserName = "Some Admin",
			};

			var singleAdmin = new MongoUser
			{
				Id = new ObjectId("5eb7022731bed923601da956"),
				UserName = "Single Admin",
			};

			var mocker = new AutoMocker();

			var userManagerMock = new Mock<IUserManager<MongoUser, ObjectId>>();
			userManagerMock.Setup(x => x.FindByIdAsync("5eb69e3e3233d47930edf61b")).ReturnsAsync(user);
			userManagerMock.Setup(x => x.GetUsersInRoleAsync(SecurityConstants.AdministratorRole)).ReturnsAsync(new[] { singleAdmin });
			userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<MongoUser>())).ReturnsAsync(IdentityResult.Success);
			mocker.Use(userManagerMock);

			var target = mocker.CreateInstance<UserService<MongoUser, ObjectId>>();

			// Act

			await target.DeleteUser("5eb69e3e3233d47930edf61b", CancellationToken.None);

			// Assert

			userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
		}
	}
}
