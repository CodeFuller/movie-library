using System;
using System.Threading;
using System.Threading.Tasks;

namespace MovieLibrary.Logic.MoviesInfo
{
	internal class StubMovieInfoProvider : IMovieInfoProvider
	{
		public async Task<MovieInfo> GetMovieInfo(Uri movieUri, CancellationToken cancellationToken)
		{
			await Task.CompletedTask;

			if (movieUri == new Uri("https://www.kinopoisk.ru/film/342/"))
			{
				return new MovieInfo
				{
					Title = "Криминальное чтиво",
					MovieUri = new Uri("https://www.kinopoisk.ru/film/342/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_1342.jpg"),
					Genres = new[]
					{
						"триллер",
						"комедия",
						"криминал",
					},
				};
			}

			throw new NotSupportedException($"Getting info for the URI '{movieUri}' is not supported");
		}
	}
}
