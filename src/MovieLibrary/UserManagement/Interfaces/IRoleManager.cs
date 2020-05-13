using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MovieLibrary.UserManagement.Interfaces
{
	internal interface IRoleManager<TRole, TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		IQueryable<TRole> Roles { get; }

		Task<IdentityResult> CreateAsync(TRole role);

		Task<TRole> FindByIdAsync(string roleId);

		Task<IdentityResult> AddClaimAsync(TRole role, Claim claim);

		Task<IList<Claim>> GetClaimsAsync(TRole role);

		Task<IdentityResult> DeleteAsync(TRole role);
	}
}
