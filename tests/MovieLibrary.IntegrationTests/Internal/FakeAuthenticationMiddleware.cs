using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MovieLibrary.IntegrationTests.Internal
{
	public class FakeAuthenticationMiddleware
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

				context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Fake Authentication"));
			}

			await next(context);
		}
	}
}
