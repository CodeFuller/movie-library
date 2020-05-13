using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class FakeAuthenticationMiddleware<TUser>
		where TUser : class
	{
		private readonly RequestDelegate next;

		private readonly ApplicationUser applicationUser;

		public FakeAuthenticationMiddleware(RequestDelegate next, ApplicationUser applicationUser)
		{
			this.next = next ?? throw new ArgumentNullException(nameof(next));
			this.applicationUser = applicationUser ?? throw new ArgumentNullException(nameof(applicationUser));
		}

		public async Task Invoke(HttpContext context, SignInManager<TUser> signInManager)
		{
			if (signInManager.IsSignedIn(context.User))
			{
				// User is already authenticated.
				return;
			}

			// We can create fake ClaimsPrincipal and fill required permission claims.
			// But instead we do it via Identity layer, for more realism.
			var user = await signInManager.UserManager.FindByNameAsync(applicationUser.Name);
			if (user == null)
			{
				throw new InvalidOperationException($"The user {applicationUser.Name} is not registered");
			}

			context.User = await signInManager.CreateUserPrincipalAsync(user);

			await next(context);
		}
	}
}
