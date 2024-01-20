using Microsoft.AspNetCore.Builder;
using MovieLibrary.Internal;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal sealed class FakeApplicationBootstrapper<TUser> : IApplicationBootstrapper
		where TUser : class
	{
		private readonly ApplicationUser authenticatedUser;

		public FakeApplicationBootstrapper(ApplicationUser authenticatedUser)
		{
			this.authenticatedUser = authenticatedUser;
		}

		public void AddAuthenticationMiddleware(IApplicationBuilder appBuilder)
		{
			if (authenticatedUser != null)
			{
				appBuilder.UseMiddleware<FakeAuthenticationMiddleware<TUser>>(authenticatedUser);
			}
		}
	}
}
