using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToGetService
	{
		Task<MovieId> AddMovie(MovieInfoModel movieInfo, CancellationToken cancellationToken);

		IQueryable<MovieToGetModel> GetAllMovies();

		Task<MovieToGetModel> GetMovie(MovieId movieId, CancellationToken cancellationToken);

		Task MoveToMoviesToSee(MovieId movieId, CancellationToken cancellationToken);

		Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken);
	}
}
