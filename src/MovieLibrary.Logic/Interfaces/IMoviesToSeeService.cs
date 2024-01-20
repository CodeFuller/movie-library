using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToSeeService
	{
		Task<MovieId> AddMovie(MovieInfoModel movieInfo, CancellationToken cancellationToken);

		IQueryable<MovieToSeeModel> GetAllMovies();

		Task<MovieToSeeModel> GetMovie(MovieId movieId, CancellationToken cancellationToken);

		Task MarkMovieAsSeen(MovieId movieId, CancellationToken cancellationToken);

		Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken);
	}
}
