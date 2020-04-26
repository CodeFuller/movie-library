using System;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	internal interface IMovieInfoProvider
	{
		Task<MovieInfoModel> GetMovieInfo(Uri movieUri, CancellationToken cancellationToken);
	}
}
