using System.Collections.Generic;
using System.Threading;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToSeeService
	{
		IAsyncEnumerable<MovieToSeeModel> GetMoviesToSee(CancellationToken cancellationToken);
	}
}
