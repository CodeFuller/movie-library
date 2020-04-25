using System;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal class KinopoiskMovieInfoProvider : IMovieInfoProvider
	{
		private readonly IHtmlContentProvider htmlContentProvider;

		private readonly IMovieInfoParser movieInfoParser;

		public KinopoiskMovieInfoProvider(IHtmlContentProvider htmlContentProvider, IMovieInfoParser movieInfoParser)
		{
			this.htmlContentProvider = htmlContentProvider ?? throw new ArgumentNullException(nameof(htmlContentProvider));
			this.movieInfoParser = movieInfoParser ?? throw new ArgumentNullException(nameof(movieInfoParser));
		}

		public async Task<MovieInfo> GetMovieInfo(Uri movieUri, CancellationToken cancellationToken)
		{
			if (!String.Equals(movieUri.Host, "www.kinopoisk.ru", StringComparison.OrdinalIgnoreCase))
			{
				throw new NotSupportedException($"Loading movie info from {movieUri} is not supported");
			}

			var htmlContent = await htmlContentProvider.GetHtmlPageContent(movieUri, cancellationToken);

			var movieInfo = movieInfoParser.ParseMovieInfo(htmlContent);

			return movieInfo;
		}
	}
}
