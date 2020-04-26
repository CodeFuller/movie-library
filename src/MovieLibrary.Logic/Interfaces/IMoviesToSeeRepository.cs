using System.Collections.Generic;
using System.Threading;
using MovieLibrary.Logic.Dto;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToSeeRepository
	{
		IAsyncEnumerable<MovieToSeeDto> ReadMoviesToSee(CancellationToken cancellationToken);
	}
}
