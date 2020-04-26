using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToSeeRepository
	{
		Task CreateMovieToSee(MovieToSeeDto movieToSee, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToSeeDto> ReadMoviesToSee(CancellationToken cancellationToken);
	}
}
