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
	}
}
