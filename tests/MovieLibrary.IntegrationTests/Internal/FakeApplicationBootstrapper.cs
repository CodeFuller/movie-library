using System;
using Microsoft.AspNetCore.Builder;
using MovieLibrary.Internal;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class FakeApplicationBootstrapper<TUser> : IApplicationBootstrapper
		where TUser : class
	{
		private readonly ApplicationUser authenticatedUser;

		public FakeApplicationBootstrapper(ApplicationUser authenticatedUser)
		{
			this.authenticatedUser = authenticatedUser ?? throw new ArgumentNullException(nameof(authenticatedUser));
		}

		public void AddAuthenticationMiddleware(IApplicationBuilder appBuilder)
		{
			appBuilder.UseMiddleware<FakeAuthenticationMiddleware<TUser>>(authenticatedUser);
		}
	}
}
