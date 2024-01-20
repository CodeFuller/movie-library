using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Services
{
	internal class MovieUniquenessChecker : IMovieUniquenessChecker
	{
		private readonly IMoviesToGetService moviesToGetService;

		private readonly IMoviesToSeeService moviesToSeeService;

		public MovieUniquenessChecker(IMoviesToGetService moviesToGetService, IMoviesToSeeService moviesToSeeService)
		{
			this.moviesToGetService = moviesToGetService ?? throw new ArgumentNullException(nameof(moviesToGetService));
			this.moviesToSeeService = moviesToSeeService ?? throw new ArgumentNullException(nameof(moviesToSeeService));
		}

		public Task<MovieUniquenessCheckResult> CheckMovie(Uri movieUri, CancellationToken cancellationToken)
		{
			var moviesToGet = moviesToGetService.GetAllMovies();
			if (moviesToGet.Any(x => x.MovieInfo.MovieUri == movieUri))
			{
				return Task.FromResult(MovieUniquenessCheckResult.ExistsInMoviesToGet);
			}

			var moviesToSee = moviesToSeeService.GetAllMovies();
			if (moviesToSee.Any(x => x.MovieInfo.MovieUri == movieUri))
			{
				return Task.FromResult(MovieUniquenessCheckResult.ExistsInMoviesToSee);
			}

			return Task.FromResult(MovieUniquenessCheckResult.MovieIsUnique);
		}
	}
}
