using Microsoft.AspNetCore.Builder;

namespace MovieLibrary.Internal
{
	public interface IApplicationBootstrapper
	{
		void AddAuthenticationMiddleware(IApplicationBuilder appBuilder);
	}
}
