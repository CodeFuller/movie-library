using Microsoft.AspNetCore.Builder;
using MovieLibrary.Internal;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class FakeApplicationBootstrapper<TUser> : IApplicationBootstrapper
		where TUser : class
	{
		private readonly ApplicationUser authenticatedUser;

		private readonly string remoteIpAddress;

		public FakeApplicationBootstrapper(ApplicationUser authenticatedUser, string remoteIpAddress)
		{
			this.authenticatedUser = authenticatedUser;
			this.remoteIpAddress = remoteIpAddress;
		}

		public void AddAuthenticationMiddleware(IApplicationBuilder appBuilder)
		{
			if (remoteIpAddress != null)
			{
				appBuilder.UseMiddleware<FakeRemoteAddressMiddleware>(remoteIpAddress);
			}

			if (authenticatedUser != null)
			{
				appBuilder.UseMiddleware<FakeAuthenticationMiddleware<TUser>>(authenticatedUser);
			}
		}
	}
}
