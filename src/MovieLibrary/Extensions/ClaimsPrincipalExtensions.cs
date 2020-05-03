using System.Security.Claims;
using MovieLibrary.Internal;

namespace MovieLibrary.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static bool CanAccessMoviesToGet(this ClaimsPrincipal principal)
		{
			return principal.IsInRole(Roles.CanAddMoviesToGet) || principal.IsInRole(Roles.CanReadMoviesToGet);
		}

		public static bool CanAccessMoviesToSee(this ClaimsPrincipal principal)
		{
			return principal.IsInRole(Roles.CanAddMoviesToSee) || principal.IsInRole(Roles.CanReadMoviesToSee);
		}
	}
}
