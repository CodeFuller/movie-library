using System;
using System.Collections.Generic;
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

		private readonly IReadOnlyCollection<string> userRoles;

		public FakeAuthenticationMiddleware(RequestDelegate next)
		{
			this.next = next ?? throw new ArgumentNullException(nameof(next));
			this.userRoles = new List<string>();
		}

		public FakeAuthenticationMiddleware(RequestDelegate next, IEnumerable<string> userRoles)
		{
			this.next = next ?? throw new ArgumentNullException(nameof(next));
			this.userRoles = userRoles?.ToList() ?? throw new ArgumentNullException(nameof(userRoles));
		}

		public async Task Invoke(HttpContext context)
		{
			if (userRoles.Any())
			{
				var claims = new[]
					{
						new Claim(ClaimTypes.Name, "Fake User"),
					}
					.Concat(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

				// Using Identity authentication type, so that SignInManager.IsSignedIn(User) returns true.
				var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
				context.User = new ClaimsPrincipal(identity);
			}

			await next(context);
		}
	}
}
