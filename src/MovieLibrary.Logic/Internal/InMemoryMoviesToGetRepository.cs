using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Internal
{
	public class InMemoryMoviesToGetRepository : IMoviesToGetRepository
	{
		private readonly List<MovieToGetDto> movies = new List<MovieToGetDto>();

		public InMemoryMoviesToGetRepository()
		{
			// Seeding some data to play with.
			movies.Add(new MovieToGetDto
			{
				Id = new MovieId("1"),
				MovieInfo = new MovieInfo
				{
					Title = "Темный рыцарь",
					MovieUri = new Uri("https://www.kinopoisk.ru/film/111543/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_111543.jpg"),
					Genres = new[] { "фантастика", "боевик", "триллер", },
				},
			});

			movies.Add(new MovieToGetDto
			{
				Id = new MovieId("2"),
				MovieInfo = new MovieInfo
				{
					Title = "Гладиатор",
					MovieUri = new Uri("https://www.kinopoisk.ru/film/474/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_474.jpg"),
					Genres = new[] { "боевик", "история", "драма", },
				},
			});

			movies.Add(new MovieToGetDto
			{
				Id = new MovieId("3"),
				MovieInfo = new MovieInfo
				{
					Title = "Большой куш",
					MovieUri = new Uri("https://www.kinopoisk.ru/film/342/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_526.jpg"),
					Genres = new[] { "криминал", "комедия", "боевик", },
				},
			});
		}

		public Task CreateMovieToGet(MovieToGetDto movieToGet, CancellationToken cancellationToken)
		{
			lock (movies)
			{
				movies.Add(movieToGet);
			}

			return Task.CompletedTask;
		}

		public IAsyncEnumerable<MovieToGetDto> ReadMoviesToGet(CancellationToken cancellationToken)
		{
			List<MovieToGetDto> moviesToReturn;

			lock (movies)
			{
				moviesToReturn = movies.ToList();
			}

			return moviesToReturn.ToAsyncEnumerable();
		}
	}
}
