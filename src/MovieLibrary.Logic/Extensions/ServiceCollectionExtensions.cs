using Microsoft.Extensions.DependencyInjection;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Internal;
using MovieLibrary.Logic.Kinopoisk;
using MovieLibrary.Logic.Services;

namespace MovieLibrary.Logic.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
		{
			services.AddSingleton<IMoviesToGetService, MoviesToGetService>();
			services.AddSingleton<IMoviesToSeeService, MoviesToSeeService>();
			services.AddSingleton<IMovieInfoService, MovieInfoService>();
			services.AddSingleton<IMovieUniquenessChecker, MovieUniquenessChecker>();

			services.AddKinopoiskMovieInfoProvider();

			services.AddSingleton<IClock, Clock>();

			return services;
		}
	}
}
