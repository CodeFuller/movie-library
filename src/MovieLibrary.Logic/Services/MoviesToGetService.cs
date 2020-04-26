﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Services
{
	internal class MoviesToGetService : IMoviesToGetService
	{
		private readonly IMovieInfoProvider movieInfoProvider;

		private readonly IMoviesToGetRepository moviesToGetRepository;

		private readonly IMoviesToSeeRepository moviesToSeeRepository;

		private readonly IClock clock;

		public MoviesToGetService(IMovieInfoProvider movieInfoProvider, IMoviesToGetRepository moviesToGetRepository, IMoviesToSeeRepository moviesToSeeRepository, IClock clock)
		{
			this.movieInfoProvider = movieInfoProvider ?? throw new ArgumentNullException(nameof(movieInfoProvider));
			this.moviesToGetRepository = moviesToGetRepository ?? throw new ArgumentNullException(nameof(moviesToGetRepository));
			this.moviesToSeeRepository = moviesToSeeRepository ?? throw new ArgumentNullException(nameof(moviesToSeeRepository));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
		}

		public async Task AddMovieToGetByUrl(Uri movieUri, CancellationToken cancellationToken)
		{
			var movieInfo = await movieInfoProvider.GetMovieInfo(movieUri, cancellationToken);

			var movieToGet = new MovieToGetDto
			{
				TimestampOfAddingToGetList = clock.Now,
				MovieInfo = movieInfo,
			};

			await moviesToGetRepository.CreateMovieToGet(movieToGet, cancellationToken);
		}

		public IAsyncEnumerable<MovieToGetModel> ReadMoviesToGet(CancellationToken cancellationToken)
		{
			return moviesToGetRepository
				.ReadAllMoviesToGet(cancellationToken)
				.Select(m => new MovieToGetModel(m.Id, m.TimestampOfAddingToGetList, m.MovieInfo))
				.OrderBy(m => m.TimestampOfAddingToGetList);
		}

		public async Task MoveToMoviesToSee(MovieId movieId, CancellationToken cancellationToken)
		{
			var movieToGet = await moviesToGetRepository.ReadMovieToGet(movieId, cancellationToken);

			var movieToSee = new MovieToSeeDto
			{
				Id = movieId,
				TimestampOfAddingToSeeList = clock.Now,
				MovieInfo = movieToGet.MovieInfo,
			};

			await moviesToSeeRepository.CreateMovieToSee(movieToSee, cancellationToken);

			await moviesToGetRepository.DeleteMovie(movieId, cancellationToken);
		}
	}
}
