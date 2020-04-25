using System;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Interfaces
{
	internal interface IMovieInfoProvider
	{
		Task<MovieInfo> GetMovieInfo(Uri movieUri, CancellationToken cancellationToken);
	}
}
