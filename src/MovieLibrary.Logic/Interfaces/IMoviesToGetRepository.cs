using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToGetRepository
	{
		Task CreateMovieToGet(NewMovieToGetModel movieToGet, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToGetModel> ReadMoviesToGet(CancellationToken cancellationToken);
	}
}
