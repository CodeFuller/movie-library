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
	internal class MoviesToGetService : IMoviesToGetService
	{
		private readonly IMovieInfoProvider movieInfoProvider;

		private readonly IMoviesToGetRepository moviesToGetRepository;

		private readonly IMoviesToSeeRepository moviesToSeeRepository;

		private readonly IClock clock;

		private readonly ILogger<MoviesToGetService> logger;

		public MoviesToGetService(IMovieInfoProvider movieInfoProvider, IMoviesToGetRepository moviesToGetRepository,
			IMoviesToSeeRepository moviesToSeeRepository, IClock clock, ILogger<MoviesToGetService> logger)
		{
			this.movieInfoProvider = movieInfoProvider ?? throw new ArgumentNullException(nameof(movieInfoProvider));
			this.moviesToGetRepository = moviesToGetRepository ?? throw new ArgumentNullException(nameof(moviesToGetRepository));
			this.moviesToSeeRepository = moviesToSeeRepository ?? throw new ArgumentNullException(nameof(moviesToSeeRepository));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task AddMovieByUrl(Uri movieUri, CancellationToken cancellationToken)
		{
			logger.LogInformation("Adding movie to get '{SourceMovieUri}' ...", movieUri);

			var movieInfo = await movieInfoProvider.GetMovieInfo(movieUri, cancellationToken);
			var movieToGet = new MovieToGetModel(clock.Now, movieInfo);

			await moviesToGetRepository.AddMovie(movieToGet, cancellationToken);
		}

		public IAsyncEnumerable<MovieToGetModel> GetAllMovies(CancellationToken cancellationToken)
		{
			return moviesToGetRepository
				.GetAllMovies(cancellationToken)
				.OrderBy(m => m.TimestampOfAddingToGetList);
		}

		public async Task MoveToMoviesToSee(MovieId movieId, CancellationToken cancellationToken)
		{
			logger.LogInformation("Moving movie {MovieId} to movies to see ...", movieId);

			var movieToGet = await moviesToGetRepository.GetMovie(movieId, cancellationToken);
			var movieToSee = new MovieToSeeModel(clock.Now, movieToGet.MovieInfo);

			await moviesToSeeRepository.AddMovie(movieToSee, cancellationToken);
			await moviesToGetRepository.DeleteMovie(movieId, cancellationToken);
		}

		public async Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken)
		{
			logger.LogInformation("Deleting movie to get {MovieId} ...", movieId);

			await moviesToGetRepository.DeleteMovie(movieId, cancellationToken);
		}
	}
}
