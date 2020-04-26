using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToGetRepository
	{
		Task CreateMovieToGet(MovieToGetDto movieToGet, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToGetDto> ReadMoviesToGet(CancellationToken cancellationToken);

		Task MoveToMoviesToSee(MovieId movieId, CancellationToken cancellationToken);
	}
}
