using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Services
{
	internal class MovieInfoService : IMovieInfoService
	{
		private readonly IMovieInfoProvider movieInfoProvider;

		private readonly ILogger<MovieInfoService> logger;

		public MovieInfoService(IMovieInfoProvider movieInfoProvider, ILogger<MovieInfoService> logger)
		{
			this.movieInfoProvider = movieInfoProvider ?? throw new ArgumentNullException(nameof(movieInfoProvider));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<MovieInfoModel> LoadMovieInfoByUrl(Uri movieUri, CancellationToken cancellationToken)
		{
			logger.LogInformation("Loading info for movie from '{SourceMovieUri}' ...", movieUri);

			return await movieInfoProvider.GetMovieInfo(movieUri, cancellationToken);
		}
	}
}
