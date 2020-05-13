using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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

		public async Task<UserModel> GetUser(string userId, CancellationToken cancellationToken)
		{
			var user = await FindUser(userId);

			return new UserModel
			{
				Id = userId,
				UserName = user.UserName,
			};
		}

		public async Task<IReadOnlyCollection<string>> GetUserRoles(string userId, CancellationToken cancellationToken)
		{
			var user = await FindUser(userId);

			return (await userManager.GetRolesAsync(user)).ToList();
		}

		public async Task AssignUserRoles(string userId, IEnumerable<string> roles, CancellationToken cancellationToken)
		{
			var user = await FindUser(userId);

			var currentRoles = await userManager.GetRolesAsync(user);

			var rolesSet = roles.ToHashSet();
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
