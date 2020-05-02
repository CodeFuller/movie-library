﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Interfaces
{
	public interface IMoviesToSeeService
	{
		Task<MovieId> AddMovieByUrl(Uri movieUri, CancellationToken cancellationToken);

		IAsyncEnumerable<MovieToSeeModel> GetAllMovies(CancellationToken cancellationToken);

		Task<MovieToSeeModel> GetMovie(MovieId movieId, CancellationToken cancellationToken);

		Task MarkMovieAsSeen(MovieId movieId, CancellationToken cancellationToken);

		Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken);
	}
}
