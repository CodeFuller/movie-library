using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using MovieLibrary.Internal;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class FakeApplicationBootstrapper : IApplicationBootstrapper
	{
		private readonly IEnumerable<string> userRoles;

		public FakeApplicationBootstrapper(IEnumerable<string> userRoles)
		{
			this.userRoles = userRoles;
		}

		public void AddAuthenticationMiddleware(IApplicationBuilder appBuilder)
		{
			if (userRoles != null)
			{
				appBuilder.UseMiddleware<FakeAuthenticationMiddleware>(userRoles);
			}
			else
			{
				appBuilder.UseMiddleware<FakeAuthenticationMiddleware>();
			}
		}
	}
}
