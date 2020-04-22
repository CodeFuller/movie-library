using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToGetRepository
	{
		Task CreateMovieToGet(MovieToGetDto movieToGet, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToGetDto> ReadMoviesToGet(CancellationToken cancellationToken);
	}
}
