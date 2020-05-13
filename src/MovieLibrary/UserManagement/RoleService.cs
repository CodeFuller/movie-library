using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MovieLibrary.Authorization;
using MovieLibrary.Exceptions;
using MovieLibrary.Extensions;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.UserManagement.Interfaces;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement
{
	internal class RoleService<TRole, TKey> : IRoleService
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly IRoleManager<TRole, TKey> roleManager;

		private readonly IIdentityRoleFactory<TRole> identityRoleFactory;

		private readonly IIdGenerator<TKey> idGenerator;

		public RoleService(IRoleManager<TRole, TKey> roleManager, IIdentityRoleFactory<TRole> identityRoleFactory, IIdGenerator<TKey> idGenerator)
		{
			this.roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
			this.identityRoleFactory = identityRoleFactory ?? throw new ArgumentNullException(nameof(identityRoleFactory));
			this.idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
		}

		public async Task<string> CreateRole(string roleName, CancellationToken cancellationToken)
		{
			var newRole = identityRoleFactory.CreateRole(roleName);
			newRole.Id = idGenerator.GenerateId();

			var result = await roleManager.CreateAsync(newRole);
			if (!result.Succeeded)
			{
				throw new UserManagementException($"Failed to create role {roleName}. {result}");
			}

			return newRole.Id.ToString();
		}

		public async IAsyncEnumerable<RoleModel> GetAllRoles([EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (var role in roleManager.Roles.ToAsyncEnumerable().WithCancellation(cancellationToken))
			{
				var claims = await roleManager.GetClaimsAsync(role);

				yield return new RoleModel
				{
					Id = role.Id.ToString(),
					RoleName = role.Name,
					Permissions = claims
						.Where(c => c.IsPermissionClaim())
						.Select(c => c.Value)
						.ToList(),
				};
			}
		}

		public async Task AssignRolePermissions(string roleId, IEnumerable<string> permissions, CancellationToken cancellationToken)
		{
			var role = await FindRole(roleId);

			// TBD: Apply the same Add/Remove logic as for user roles.
			foreach (var permission in permissions)
			{
				var result = await roleManager.AddClaimAsync(role, new Claim(SecurityConstants.PermissionClaimType, permission));
				if (!result.Succeeded)
				{
					throw new UserManagementException($"Failed to add claim permission {permission} for role {role.Name}. {result}");
				}
			}
		}

		public async Task DeleteRole(string roleId, CancellationToken cancellationToken)
		{
			// TBD: We should not allow deletion of built-in role.
			var role = await FindRole(roleId);

			var result = await roleManager.DeleteAsync(role);
			if (!result.Succeeded)
			{
				throw new UserManagementException($"Failed to delete the role. {result}");
			}
		}

		private async Task<TRole> FindRole(string roleId)
		{
			var role = await roleManager.FindByIdAsync(roleId);
			if (role == null)
			{
				throw new UserManagementException($"The role with id {roleId} was not found");
			}

			return role;
		}
	}
}
