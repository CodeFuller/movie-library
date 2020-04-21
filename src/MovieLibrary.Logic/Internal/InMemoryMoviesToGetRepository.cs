using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Internal
{
	public class InMemoryMoviesToGetRepository : IMoviesToGetRepository
	{
		private readonly List<MovieToGetModel> movies = new List<MovieToGetModel>();

		public InMemoryMoviesToGetRepository()
		{
			// Seeding some data to play with.
			movies.Add(new MovieToGetModel
			{
				Id = new MovieId("1"),
				Title = "Pulp Fiction",
			});

			movies.Add(new MovieToGetModel
			{
				Id = new MovieId("2"),
				Title = "Gladiator",
			});

			movies.Add(new MovieToGetModel
			{
				Id = new MovieId("3"),
				Title = "Snatch",
			});
		}

		public Task CreateMovieToGet(NewMovieToGetModel movieToGet, CancellationToken cancellationToken)
		{
			var movie = new MovieToGetModel
			{
				Id = new MovieId(Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)),
				Title = movieToGet.Title,
			};

			lock (movies)
			{
				movies.Add(movie);
			}

			return Task.CompletedTask;
		}

		public IAsyncEnumerable<MovieToGetModel> ReadMoviesToGet(CancellationToken cancellationToken)
		{
			List<MovieToGetModel> moviesToReturn;

			lock (movies)
			{
				moviesToReturn = movies.ToList();
			}

			return moviesToReturn.ToAsyncEnumerable();
		}
	}
}
