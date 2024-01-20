using Microsoft.AspNetCore.Builder;

namespace MovieLibrary.Internal
{
	internal class ApplicationBootstrapper : IApplicationBootstrapper
	{
		public void AddAuthenticationMiddleware(IApplicationBuilder appBuilder)
		{
			appBuilder.UseAuthentication();
		}
	}
}
