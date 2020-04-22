using System;
using System.Threading;
using System.Threading.Tasks;

namespace MovieLibrary.Logic.MoviesInfo
{
	internal interface IMovieInfoProvider
	{
		Task<MovieInfo> GetMovieInfo(Uri movieUri, CancellationToken cancellationToken);
	}
}
