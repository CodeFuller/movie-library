using Microsoft.Extensions.DependencyInjection;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Internal;
using MovieLibrary.Logic.MoviesInfo;
using MovieLibrary.Logic.Services;

namespace MovieLibrary.Logic.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
		{
			services.AddSingleton<IMoviesToGetService, MoviesToGetService>();
			services.AddSingleton<IMovieInfoProvider, StubMovieInfoProvider>();

			services.AddSingleton<IMoviesToGetRepository, InMemoryMoviesToGetRepository>();

			return services;
		}
	}
}
