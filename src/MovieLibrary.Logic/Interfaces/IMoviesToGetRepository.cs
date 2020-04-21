using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToGetRepository
	{
		Task CreateMovieToGet(CreateMovieToGetDto movieToGet, CancellationToken cancellationToken);

		IAsyncEnumerable<ReadMovieToGetDto> ReadMoviesToGet(CancellationToken cancellationToken);
	}
}
