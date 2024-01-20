using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal static class KinopoiskServiceCollectionExtensions
	{
		public static IServiceCollection AddKinopoiskMovieInfoProvider(this IServiceCollection services)
		{
			services.AddSingleton<IFilmDataToMovieInfoConverter, FilmDataToMovieInfoConverter>();

			services
				.AddHttpClient<IMovieInfoProvider, KinopoiskApiMovieInfoProvider>()
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
				{
					AllowAutoRedirect = false,
					AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
				});

			return services;
		}
	}
}
