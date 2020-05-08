using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToSeeRepository
	{
		Task<MovieId> AddMovie(MovieToSeeModel movie, CancellationToken cancellationToken);

		IQueryable<MovieToSeeModel> GetAllMovies();

		Task<MovieToSeeModel> GetMovie(MovieId movieId, CancellationToken cancellationToken);

		Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken);
	}
}
