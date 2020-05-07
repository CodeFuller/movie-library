using System;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMovieInfoService
	{
		Task<MovieInfoModel> LoadMovieInfoByUrl(Uri movieUri, CancellationToken cancellationToken);
	}
}
