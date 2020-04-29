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
		private readonly IMovieInfoProvider movieInfoProvider;

		private readonly IMoviesToSeeRepository repository;

		private readonly IClock clock;

		private readonly ILogger<MoviesToSeeService> logger;

		public MoviesToSeeService(IMovieInfoProvider movieInfoProvider, IMoviesToSeeRepository repository, IClock clock, ILogger<MoviesToSeeService> logger)
		{
			this.movieInfoProvider = movieInfoProvider ?? throw new ArgumentNullException(nameof(movieInfoProvider));
			this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task AddMovieByUrl(Uri movieUri, CancellationToken cancellationToken)
		{
			logger.LogInformation("Adding movie to see '{SourceMovieUri}' ...", movieUri);

			var movieInfo = await movieInfoProvider.GetMovieInfo(movieUri, cancellationToken);
			var movieToSee = new MovieToSeeModel(clock.Now, movieInfo);

			await repository.AddMovie(movieToSee, cancellationToken);
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
