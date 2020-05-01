using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToSeeRepository
	{
		Task<MovieId> AddMovie(MovieToSeeModel movie, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToSeeModel> GetAllMovies(CancellationToken cancellationToken);

		Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken);
	}
}
