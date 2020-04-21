using Microsoft.Extensions.DependencyInjection;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Internal;

namespace MovieLibrary.Logic.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
		{
			services.AddSingleton<IMoviesToGetRepository, InMemoryMoviesToGetRepository>();

			return services;
		}
	}
}
