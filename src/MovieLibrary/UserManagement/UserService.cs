using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MovieLibrary.Authorization;
using MovieLibrary.Exceptions;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.UserManagement.Interfaces;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement
{
	internal class UserService<TUser, TKey> : IUserService
		where TUser : IdentityUser<TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly IUserManager<TUser, TKey> userManager;

		private readonly IIdentityUserFactory<TUser> identityUserFactory;

		private readonly IIdGenerator<TKey> idGenerator;

		public UserService(IUserManager<TUser, TKey> userManager, IIdentityUserFactory<TUser> identityUserFactory, IIdGenerator<TKey> idGenerator)
		{
			this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			this.identityUserFactory = identityUserFactory ?? throw new ArgumentNullException(nameof(identityUserFactory));
			this.idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
		}

		public async Task<string> CreateUser(NewUserModel user, CancellationToken cancellationToken)
		{
			var userEmail = user.Email;

			var newUser = identityUserFactory.CreateUser(userEmail);
			newUser.Id = idGenerator.GenerateId();
			newUser.Email = userEmail;

			var result = await userManager.CreateAsync(newUser, user.Password);
			if (!result.Succeeded)
			{
				throw new UserManagementException($"Failed to create the user. {result}");
			}

			return newUser.Id.ToString();
		}

		public async Task CreateDefaultAdministrator(CancellationToken cancellationToken)
		{
			var newUser = new NewUserModel
			{
				Email = SecurityConstants.DefaultAdministratorEmail,
				Password = SecurityConstants.DefaultAdministratorPassword,
			};

			var userId = await CreateUser(newUser, cancellationToken);

			await AssignUserRoles(userId, new[] { SecurityConstants.AdministratorRole }, checkIfCanBeEdited: false);
		}

		public async IAsyncEnumerable<UserModel> GetAllUsers([EnumeratorCancellation] CancellationToken cancellationToken)
		{
			var adminUsers = await GetAdminUsers();

			await foreach (var user in userManager.Users.ToAsyncEnumerable().WithCancellation(cancellationToken))
			{
				yield return CreateUserModel(user, adminUsers);
			}
		}

		public async Task<UserModel> GetUser(string userId, CancellationToken cancellationToken)
		{
			var user = await FindUser(userId);
			var adminUsers = await GetAdminUsers();

			return CreateUserModel(user, adminUsers);
		}

		public async Task<IReadOnlyCollection<UserRoleModel>> GetUserRoles(string userId, CancellationToken cancellationToken)
		{
			var user = await FindUser(userId);
			var roles = await userManager.GetRolesAsync(user);

			var adminRoleIsReadOnly = roles.Contains(SecurityConstants.AdministratorRole) && (await GetAdminUsers()).Count == 1;

			return roles.Select(role => new UserRoleModel
				{
					RoleName = role,
					ReadOnly = String.Equals(role, SecurityConstants.AdministratorRole, StringComparison.Ordinal) && adminRoleIsReadOnly,
				})
				.ToList();
		}

		public Task AssignUserRoles(string userId, IEnumerable<string> roles, CancellationToken cancellationToken)
		{
			return AssignUserRoles(userId, roles, checkIfCanBeEdited: true);
		}

		public async Task AssignUserRoles(string userId, IEnumerable<string> roles, bool checkIfCanBeEdited)
		{
			var user = await FindUser(userId);

			if (checkIfCanBeEdited && !UserCanBeEdited(user))
			{
				throw new UserManagementException($"The user {user.UserName} can not be modified");
			}

			var currentRoles = await userManager.GetRolesAsync(user);

			var rolesSet = roles.ToHashSet();
			var rolesToAdd = rolesSet.Where(r => !currentRoles.Contains(r)).ToList();
			var rolesToRemove = currentRoles.Where(r => !rolesSet.Contains(r)).ToList();

			if (rolesToRemove.Contains(SecurityConstants.AdministratorRole) && (await GetAdminUsers()).Count == 1)
			{
				throw new UserManagementException("The administrator role can not be unassigned from the last administrator");
			}

			await AddUserRoles(user, rolesToAdd);
			await RemoveUserRoles(user, rolesToRemove);
		}

		public async Task DeleteUser(string userId, CancellationToken cancellationToken)
		{
			var user = await FindUser(userId);
			var adminUsers = await GetAdminUsers();

			if (!UserCanBeDeleted(user, adminUsers))
			{
				throw new UserManagementException($"The user {user.UserName} can not be deleted");
			}

			await DeleteUser(user);
		}

		public async Task Clear(CancellationToken cancellationToken)
		{
			foreach (var role in userManager.Users.ToList())
			{
				await DeleteUser(role);
			}
		}

		private async Task DeleteUser(TUser user)
		{
			var result = await userManager.DeleteAsync(user);
			if (!result.Succeeded)
			{
				throw new UserManagementException($"Failed to delete the user. {result}");
			}
		}

		private async Task<TUser> FindUser(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				throw new UserManagementException($"The user with id {userId} was not found");
			}

			return user;
		}

		private static bool UserCanBeEdited(TUser user)
		{
			return !String.Equals(user.UserName, SecurityConstants.DefaultAdministratorEmail, StringComparison.Ordinal);
		}

		private static bool UserCanBeDeleted(TUser user, HashSet<TKey> adminUsers)
		{
			return adminUsers.Count > 1 || !adminUsers.Contains(user.Id);
		}

		private async Task<HashSet<TKey>> GetAdminUsers()
		{
			return (await userManager.GetUsersInRoleAsync(SecurityConstants.AdministratorRole))
				.Select(u => u.Id)
				.ToHashSet();
		}

		private static UserModel CreateUserModel(TUser user, HashSet<TKey> adminUsers)
		{
			return new UserModel
			{
				Id = user.Id.ToString(),
				UserName = user.UserName,
				CanBeEdited = UserCanBeEdited(user),
				CanBeDeleted = UserCanBeDeleted(user, adminUsers),
			};
		}

		private async Task AddUserRoles(TUser user, IReadOnlyCollection<string> rolesToAdd)
		{
			if (!rolesToAdd.Any())
			{
				return;
			}

			var result = await userManager.AddToRolesAsync(user, rolesToAdd);
			if (!result.Succeeded)
			{
				throw new UserManagementException($"Failed to add roles for the user. {result}");
			}
		}

		private async Task RemoveUserRoles(TUser user, IReadOnlyCollection<string> rolesToRemove)
		{
			if (!rolesToRemove.Any())
			{
				return;
			}

			var result = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
			if (!result.Succeeded)
			{
				throw new UserManagementException($"Failed to remove roles for the user. {result}");
			}
		}
	}
}
