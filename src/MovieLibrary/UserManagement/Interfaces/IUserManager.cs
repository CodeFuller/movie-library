using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MovieLibrary.UserManagement.Interfaces
{
	internal interface IUserManager<TUser, TKey>
		where TUser : IdentityUser<TKey>
		where TKey : IEquatable<TKey>
	{
		IQueryable<TUser> Users { get; }

		Task<IdentityResult> CreateAsync(TUser user, string password);

		Task<TUser> FindByIdAsync(string userId);

		Task<IList<string>> GetRolesAsync(TUser user);

		Task<IList<TUser>> GetUsersInRoleAsync(string roleName);

		Task<IdentityResult> AddToRolesAsync(TUser user, IEnumerable<string> roles);

		Task<IdentityResult> RemoveFromRolesAsync(TUser user, IEnumerable<string> roles);

		Task<IdentityResult> DeleteAsync(TUser user);
	}
}
