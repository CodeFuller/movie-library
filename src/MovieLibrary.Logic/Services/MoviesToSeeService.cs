using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Services
{
	internal class MoviesToSeeService : IMoviesToSeeService
	{
		private readonly IMoviesToSeeRepository repository;

		private readonly ILogger<MoviesToSeeService> logger;

		public MoviesToSeeService(IMoviesToSeeRepository repository, ILogger<MoviesToSeeService> logger)
		{
			this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public IAsyncEnumerable<MovieToSeeModel> GetAllMovies(CancellationToken cancellationToken)
		{
			return repository
				.GetAllMovies(cancellationToken)
				.Select(m => new MovieToSeeModel(m.Id, m.TimestampOfAddingToSeeList, m.MovieInfo))
				.OrderBy(m => m.TimestampOfAddingToSeeList);
		}

		public async Task MarkMovieAsSeen(MovieId movieId, CancellationToken cancellationToken)
		{
			logger.LogInformation("Marking movie {MovieId} as seen ...", movieId);

			await repository.DeleteMovie(movieId, cancellationToken);
		}
	}
}
