using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToGetRepository
	{
		Task<MovieId> AddMovie(MovieToGetModel movie, CancellationToken cancellationToken);

		IQueryable<MovieToGetModel> GetAllMovies();

		Task<MovieToGetModel> GetMovie(MovieId movieId, CancellationToken cancellationToken);

		Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken);
	}
}
