using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.Logic.Internal
{
	public class InMemoryMoviesToGetRepository : IMoviesToGetRepository
	{
		private readonly List<ReadMovieToGetDto> movies = new List<ReadMovieToGetDto>();

		public InMemoryMoviesToGetRepository()
		{
			// Seeding some data to play with.
			movies.Add(new ReadMovieToGetDto
			{
				Id = new MovieId("1"),
				Title = "Pulp Fiction",
			});

			movies.Add(new ReadMovieToGetDto
			{
				Id = new MovieId("2"),
				Title = "Gladiator",
			});

			movies.Add(new ReadMovieToGetDto
			{
				Id = new MovieId("3"),
				Title = "Snatch",
			});
		}

		public Task CreateMovieToGet(CreateMovieToGetDto movieToGet, CancellationToken cancellationToken)
		{
			var movie = new ReadMovieToGetDto
			{
				Id = new MovieId(Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)),
			};

			lock (movies)
			{
				movies.Add(movie);
			}

			return Task.CompletedTask;
		}

		public IAsyncEnumerable<ReadMovieToGetDto> ReadMoviesToGet(CancellationToken cancellationToken)
		{
			List<ReadMovieToGetDto> moviesToReturn;

			lock (movies)
			{
				moviesToReturn = movies.ToList();
			}

			return moviesToReturn.ToAsyncEnumerable();
		}
	}
}
