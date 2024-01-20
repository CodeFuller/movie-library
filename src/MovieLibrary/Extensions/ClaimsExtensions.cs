using System;
using System.Security.Claims;
using MovieLibrary.Authorization;

namespace MovieLibrary.Extensions
{
	public static class ClaimsExtensions
	{
		public static bool IsPermissionClaim(this Claim claim)
		{
			return String.Equals(claim.Type, SecurityConstants.PermissionClaimType, StringComparison.Ordinal);
		}

		public static bool HasPermission(this ClaimsPrincipal principal, string permission)
		{
			return principal.HasClaim(SecurityConstants.PermissionClaimType, permission);
		}

		public static bool CanAccessMoviesToGet(this ClaimsPrincipal principal)
		{
			return principal.HasPermission(ApplicationPermissions.MoviesToGet.Add) || principal.HasPermission(ApplicationPermissions.MoviesToGet.Read);
		}

		public static bool CanAccessMoviesToSee(this ClaimsPrincipal principal)
		{
			return principal.HasPermission(ApplicationPermissions.MoviesToSee.Add) || principal.HasPermission(ApplicationPermissions.MoviesToSee.Read);
		}

		public static bool CanManagerUsers(this ClaimsPrincipal principal)
		{
			return principal.IsInRole(SecurityConstants.AdministratorRole);
		}
	}
}
