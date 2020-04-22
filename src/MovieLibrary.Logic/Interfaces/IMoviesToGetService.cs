using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToGetService
	{
		Task AddMovieToGetByUrl(Uri movieUri, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToGetDto> GetMoviesToGet(CancellationToken cancellationToken);
	}
}
