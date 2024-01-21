using System;
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
		private readonly IMoviesToGetRepository moviesToGetRepository;

		private readonly IMoviesToSeeRepository moviesToSeeRepository;

		private readonly IClock clock;

		private readonly ILogger<MoviesToGetService> logger;

		public MoviesToGetService(IMoviesToGetRepository moviesToGetRepository, IMoviesToSeeRepository moviesToSeeRepository, IClock clock, ILogger<MoviesToGetService> logger)
		{
			this.moviesToGetRepository = moviesToGetRepository ?? throw new ArgumentNullException(nameof(moviesToGetRepository));
			this.moviesToSeeRepository = moviesToSeeRepository ?? throw new ArgumentNullException(nameof(moviesToSeeRepository));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<MovieId> AddMovie(MovieInfoModel movieInfo, string reference, CancellationToken cancellationToken)
		{
			logger.LogInformation("Adding movie to get '{SourceMovieUri}' ...", movieInfo.MovieUri);

			var movieToGet = new MovieToGetModel
			{
				TimestampOfAddingToGetList = clock.Now,
				MovieInfo = movieInfo,
				Reference = reference,
			};

			return await moviesToGetRepository.AddMovie(movieToGet, cancellationToken);
		}

		public IQueryable<MovieToGetModel> GetAllMovies()
		{
			return moviesToGetRepository
				.GetAllMovies()
				.OrderBy(m => m.TimestampOfAddingToGetList);
		}

		public Task<MovieToGetModel> GetMovie(MovieId movieId, CancellationToken cancellationToken)
		{
			return moviesToGetRepository.GetMovie(movieId, cancellationToken);
		}

		public async Task MoveToMoviesToSee(MovieId movieId, CancellationToken cancellationToken)
		{
			logger.LogInformation("Moving movie {MovieId} to movies to see ...", movieId);

			var movieToGet = await moviesToGetRepository.GetMovie(movieId, cancellationToken);
			var movieToSee = new MovieToSeeModel
			{
				TimestampOfAddingToSeeList = clock.Now,
				MovieInfo = movieToGet.MovieInfo,
				Reference = movieToGet.Reference,
			};

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
