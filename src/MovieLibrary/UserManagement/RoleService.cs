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
#pragma warning disable CA1508 // Avoid dead conditional code
			await foreach (var role in roleManager.Roles.ToAsyncEnumerable().WithCancellation(cancellationToken))
#pragma warning restore CA1508 // Avoid dead conditional code
			{
				yield return new RoleModel
				{
					Id = role.Id.ToString(),
					RoleName = role.Name,
					ReadOnly = RoleIsReadOnly(role),
				};
			}
		}

		private static bool RoleIsReadOnly(TRole role)
		{
			return SecurityConstants.IsAdministratorRole(role.Name);
		}

		public async Task<RoleModel> GetRole(string roleId, CancellationToken cancellationToken)
		{
			var role = await FindRole(roleId);

			return new RoleModel
			{
				Id = roleId,
				RoleName = role.Name,
				ReadOnly = RoleIsReadOnly(role),
			};
		}

		public async Task<IReadOnlyCollection<string>> GetRolePermissions(string roleId, CancellationToken cancellationToken)
		{
			var role = await FindRole(roleId);
			var claims = await roleManager.GetClaimsAsync(role);

			return ExtractPermissions(claims)
				.ToList();
		}

		public async Task AssignRolePermissions(string roleId, IEnumerable<string> permissions, CancellationToken cancellationToken)
		{
			var role = await FindRole(roleId);

			if (RoleIsReadOnly(role))
			{
				throw new UserManagementException($"The role {role.Name} can not be modified");
			}

			var roleClaims = await roleManager.GetClaimsAsync(role);
			var currentPermissions = ExtractPermissions(roleClaims);

			var permissionsSet = permissions.ToHashSet();
			var permissionsToAdd = permissionsSet.Where(r => !currentPermissions.Contains(r)).ToList();
			var permissionsToRemove = currentPermissions.Where(r => !permissionsSet.Contains(r)).ToList();

			await AddRolePermissions(role, permissionsToAdd);
			await RemoveRolePermissions(role, permissionsToRemove);
		}

		public async Task DeleteRole(string roleId, CancellationToken cancellationToken)
		{
			var role = await FindRole(roleId);

			if (RoleIsReadOnly(role))
			{
				throw new UserManagementException($"The role {role.Name} can not be deleted");
			}

			await DeleteRole(role);
		}

		public async Task Clear(CancellationToken cancellationToken)
		{
			foreach (var role in roleManager.Roles.ToList())
			{
				await DeleteRole(role);
			}
		}

		private async Task DeleteRole(TRole role)
		{
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

		private static IEnumerable<string> ExtractPermissions(IEnumerable<Claim> claims)
		{
			return claims
				.Where(c => c.IsPermissionClaim())
				.Select(c => c.Value);
		}

		private async Task AddRolePermissions(TRole role, IEnumerable<string> permissionsToAdd)
		{
			foreach (var permission in permissionsToAdd)
			{
				var claim = new Claim(SecurityConstants.PermissionClaimType, permission);

				var result = await roleManager.AddClaimAsync(role, claim);
				if (!result.Succeeded)
				{
					throw new UserManagementException($"Failed to add permission claim {permission} for role {role.Name}. {result}");
				}
			}
		}

		private async Task RemoveRolePermissions(TRole role, IEnumerable<string> permissionsToRemove)
		{
			foreach (var permission in permissionsToRemove)
			{
				var claim = new Claim(SecurityConstants.PermissionClaimType, permission);

				var result = await roleManager.RemoveClaimAsync(role, claim);
				if (!result.Succeeded)
				{
					throw new UserManagementException($"Failed to remove permission claim {permission} for role {role.Name}. {result}");
				}
			}
		}
	}
}
