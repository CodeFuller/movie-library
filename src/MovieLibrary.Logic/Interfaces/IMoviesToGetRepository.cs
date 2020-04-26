using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToGetRepository
	{
		Task AddMovie(MovieToGetDto movie, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToGetDto> GetAllMovies(CancellationToken cancellationToken);

		Task<MovieToGetDto> GetMovie(MovieId movieId, CancellationToken cancellationToken);

		Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken);
	}
}
