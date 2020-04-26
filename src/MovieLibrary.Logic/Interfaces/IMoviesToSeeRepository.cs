using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToSeeRepository
	{
		Task AddMovie(MovieToSeeDto movie, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToSeeDto> GetAllMovies(CancellationToken cancellationToken);
	}
}
