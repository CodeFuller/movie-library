﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToGetService
	{
		Task AddMovieToGetByUrl(Uri movieUri, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToGetModel> GetMoviesToGet(CancellationToken cancellationToken);
	}
}
