﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.Logic.Services
{
	internal class MoviesToGetService : IMoviesToGetService
	{
		private readonly IMovieInfoProvider movieInfoProvider;

		private readonly IMoviesToGetRepository repository;

		public MoviesToGetService(IMovieInfoProvider movieInfoProvider, IMoviesToGetRepository repository)
		{
			this.movieInfoProvider = movieInfoProvider ?? throw new ArgumentNullException(nameof(movieInfoProvider));
			this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		public async Task AddMovieToGetByUrl(Uri movieUri, CancellationToken cancellationToken)
		{
			var movieInfo = await movieInfoProvider.GetMovieInfo(movieUri, cancellationToken);

			var movieToGet = new MovieToGetDto
			{
				MovieInfo = movieInfo,
			};

			await repository.CreateMovieToGet(movieToGet, cancellationToken);
		}

		public IAsyncEnumerable<MovieToGetDto> GetMoviesToGet(CancellationToken cancellationToken)
		{
			return repository.ReadMoviesToGet(cancellationToken);
		}
	}
}
