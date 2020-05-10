using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MovieLibrary.Exceptions;
using MovieLibrary.Logic.Exceptions;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement
{
	internal class UserService<TUser, TRole, TKey> : IUserService
		where TUser : IdentityUser<TKey>, new()
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly IUserManager<TUser, TKey> userManager;
		private readonly IRoleManager<TRole, TKey> roleManager;

		private readonly IUserStore<TUser> userStore;

		private readonly IIdGenerator<TKey> idGenerator;

		public UserService(IUserManager<TUser, TKey> userManager, IRoleManager<TRole, TKey> roleManager, IUserStore<TUser> userStore, IIdGenerator<TKey> idGenerator)
		{
			this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			this.roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
			this.userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
			this.idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
		}

		public async Task<string> CreateUser(NewUserModel user, CancellationToken cancellationToken)
		{
			var userEmail = user.Email;

			var newUser = new TUser
			{
				Id = idGenerator.GenerateId(),
				Email = userEmail,
			};

			await userStore.SetUserNameAsync(newUser, userEmail, cancellationToken);

			var result = await userManager.CreateAsync(newUser, user.Password);
			if (!result.Succeeded)
			{
				throw new UserUpdateFailedException($"Failed to create the user. {result}");
			}

			return newUser.Id.ToString();
		}

		public async IAsyncEnumerable<UserModel> GetAllUsers([EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (var user in userManager.Users.ToAsyncEnumerable().WithCancellation(cancellationToken))
			{
				yield return new UserModel
				{
					Id = user.Id.ToString(),
					UserName = user.UserName,
				};
			}
		}

		public async Task<UserDetailsModel> GetUser(string userId, CancellationToken cancellationToken)
		{
			var user = await FindUser(userId);

			var roles = await userManager.GetRolesAsync(user);
			var allRoles = await roleManager.Roles.ToAsyncEnumerable().ToListAsync(cancellationToken);

			return new UserDetailsModel
			{
				Id = userId,
				UserName = user.UserName,
				UserPermissions = roles?.ToList() ?? new List<string>(),
				AllPermissions = allRoles.Select(r => r.Name).ToList(),
			};
		}

		public async Task AssignUserPermissions(string userId, IEnumerable<string> permissions, CancellationToken cancellationToken)
		{
			var user = await FindUser(userId);

			var currentRoles = await userManager.GetRolesAsync(user);

			var rolesSet = permissions.ToHashSet();
			var rolesToAdd = rolesSet.Where(r => !currentRoles.Contains(r)).ToList();
			var rolesToRemove = currentRoles.Where(r => !rolesSet.Contains(r)).ToList();

			await AddUserRoles(user, rolesToAdd);
			await RemoveUserRoles(user, rolesToRemove);
		}

		public async Task DeleteUser(string userId, CancellationToken cancellationToken)
		{
			var user = await FindUser(userId);

			var result = await userManager.DeleteAsync(user);
			if (!result.Succeeded)
			{
				throw new UserUpdateFailedException($"Failed to delete the user. {result}");
			}
		}

		private async Task<TUser> FindUser(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				throw new NotFoundException($"The user with id {userId} was not found");
			}

			return user;
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
				throw new UserUpdateFailedException($"Failed to add roles for the user. {result}");
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
				throw new UserUpdateFailedException($"Failed to remove roles for the user. {result}");
			}
		}
	}
}
