using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MovieLibrary.Internal;

namespace MovieLibrary.Authorization
{
	internal static class AuthorizationServiceCollectionExtensions
	{
		public static IServiceCollection AddPermissionBasedAuthorization(this IServiceCollection services)
		{
			services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
			services.AddScoped<IAuthorizationHandler, DefaultAdminAuthorizationHandler>();
			services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
			services.AddScoped<IApplicationInitializer, UsersInitializer>();

			return services;
		}
	}
}
