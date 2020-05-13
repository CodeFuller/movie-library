using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class FakeAuthenticationMiddleware
	{
		private readonly RequestDelegate next;

		private readonly ApplicationUser authenticatedUser;

		public FakeAuthenticationMiddleware(RequestDelegate next, ApplicationUser authenticatedUser)
		{
			this.next = next ?? throw new ArgumentNullException(nameof(next));
			this.authenticatedUser = authenticatedUser ?? throw new ArgumentNullException(nameof(authenticatedUser));
		}

		public async Task Invoke(HttpContext context)
		{
			// TBD: Load user via UserService for more realism.
			var claims = new[]
				{
					new Claim(ClaimTypes.Name, "Fake User"),
				}
				.Concat(authenticatedUser.Roles.Select(role => new Claim(ClaimTypes.Role, role)))
				.Concat(authenticatedUser.Permissions.Select(permission => new Claim("permission", permission)));

			// Using Identity authentication type, so that SignInManager.IsSignedIn(User) returns true.
			var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
			context.User = new ClaimsPrincipal(identity);

			await next(context);
		}
	}
}
