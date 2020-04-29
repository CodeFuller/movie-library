using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToGetService
	{
		Task AddMovieByUrl(Uri movieUri, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToGetModel> GetAllMovies(CancellationToken cancellationToken);

		Task MoveToMoviesToSee(MovieId movieId, CancellationToken cancellationToken);

		Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken);
	}
}
