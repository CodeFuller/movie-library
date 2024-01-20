using System;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMovieUniquenessChecker
	{
		Task<MovieUniquenessCheckResult> CheckMovie(Uri movieUri, CancellationToken cancellationToken);
	}
}
