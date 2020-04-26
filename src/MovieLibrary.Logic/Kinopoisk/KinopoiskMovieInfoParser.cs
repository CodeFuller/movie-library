using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal class KinopoiskMovieInfoParser : IMovieInfoParser
	{
		private static readonly Regex TitleRegex = new Regex(@"<span class=""moviename-title-wrapper"">(.+?)</span>", RegexOptions.Compiled);
		private static readonly Regex YearRegex = new Regex(@"<a href=""/lists/navigator/(\d+)/\?quick_filters=films"" title="""">\1</a>", RegexOptions.Compiled);
		private static readonly Regex MovieUriRegex = new Regex(@"<link rel=""canonical"" href=""(https?://.+?)"" />", RegexOptions.Compiled);
		private static readonly Regex PosterUriRegex = new Regex(@"<link rel=""image_src"" href=""(https?://.+?)"" />", RegexOptions.Compiled);
		private static readonly Regex DirectorsRegex = new Regex(@"<td itemprop=""director"">\s*(?:<a href=""/name/\d+/"">(.+?)</a>(?:,\s*)?)+</td>", RegexOptions.Compiled);
		private static readonly Regex CastRegex = new Regex(@"<li itemprop=""actors""><a href=""/name/\d+/"">(.+?)</a></li>", RegexOptions.Compiled);
		private static readonly Regex RatingValueRegex = new Regex(@"<span class=""rating_ball"">(\d+\.\d+)</span>", RegexOptions.Compiled);
		private static readonly Regex RatingCountRegex = new Regex(@"<meta itemprop=""ratingCount"" content=""(\d+)"" />", RegexOptions.Compiled);
		private static readonly Regex DurationRegex = new Regex(@"<td class=""time"" id=""runtime"">(\d+) мин\. ", RegexOptions.Compiled);
		private static readonly Regex GenresRegex = new Regex(@"<span itemprop=""genre"">\s*(?:<a href=""/lists/navigator/.+?/\?quick_filters=films"">(.+?)</a>(?:,\s*)?)+</span>", RegexOptions.Compiled);
		private static readonly Regex SummaryRegex = new Regex(@"<span class=""_reachbanner_""><div class=""brand_words film-synopsys"" itemprop=""description"">(.+?)</div></span>", RegexOptions.Compiled);

		private readonly ILogger<KinopoiskMovieInfoParser> logger;

		public KinopoiskMovieInfoParser(ILogger<KinopoiskMovieInfoParser> logger)
		{
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public MovieInfoModel ParseMovieInfo(string content, Uri sourceUri)
		{
			string title = null;
			string yearText = null;
			string movieUri = null;
			string posterUri = null;
			List<string> directors = null;
			List<string> cast = null;

			string ratingValue = null;
			string ratingCount = null;

			string duration = null;
			List<string> genres = null;

			string summary = null;

			foreach (var line in content.Split("\n"))
			{
				title ??= ParseValue(line, TitleRegex);
				yearText ??= ParseValue(line, YearRegex);
				movieUri ??= ParseValue(line, MovieUriRegex);
				posterUri ??= ParseValue(line, PosterUriRegex);
				directors ??= ParseMultipleValues(line, DirectorsRegex);
				cast ??= ParseMultipleValues(line, CastRegex);

				ratingValue ??= ParseValue(line, RatingValueRegex);
				ratingCount ??= ParseValue(line, RatingCountRegex);

				duration ??= ParseValue(line, DurationRegex);
				genres ??= ParseMultipleValues(line, GenresRegex);

				summary ??= ParseValue(line, SummaryRegex);
			}

			if (String.Equals(posterUri, "https://st.kp.yandex.net/images/movies/poster_none.png", StringComparison.Ordinal))
			{
				posterUri = null;
			}

			var movieInfo = new MovieInfoModel
			{
				Title = title,
				Year = ParseInt(yearText, "year"),
				MovieUri = ParseUri(movieUri),
				PosterUri = ParseUri(posterUri),
				Directors = directors,
				Cast = cast,
				Rating = ParseRating(ratingValue, ratingCount),
				Duration = ParseDuration(duration),
				Genres = genres,
				Summary = SanitizeSummary(summary),
			};

			CheckMovieInfoForRequiredData(movieInfo, sourceUri);

			return movieInfo;
		}

		private static void CheckMovieInfoForRequiredData(MovieInfoModel movieInfo, Uri movieUri)
		{
			if (String.IsNullOrWhiteSpace(movieInfo.Title))
			{
				throw new InvalidOperationException($"Failed to parse movie title from '{movieUri}'");
			}

			if (movieInfo.MovieUri == null)
			{
				throw new InvalidOperationException($"Failed to parse movie URI from '{movieUri}'");
			}
		}

		private static string ParseValue(string text, Regex regex)
		{
			var match = regex.Match(text);
			return match.Success ? match.Groups[1].Value : null;
		}

		private static List<string> ParseMultipleValues(string text, Regex regex)
		{
			var matches = regex.Matches(text);
			if (matches.Count == 0)
			{
				return null;
			}

			return matches.SelectMany(m => m.Groups[1].Captures.Select(c => c.Value)).ToList();
		}

		private int? ParseInt(string text, string valueTitle)
		{
			if (text == null)
			{
				return null;
			}

			if (Int32.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var parsedValue))
			{
				return parsedValue;
			}

			logger.LogWarning($"Failed to parse {valueTitle} from '{{ValueToParse}}'", text);
			return null;
		}

		private decimal? ParseDecimal(string text, string valueTitle)
		{
			if (text == null)
			{
				return null;
			}

			if (Decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsedValue))
			{
				return parsedValue;
			}

			logger.LogWarning($"Failed to parse {valueTitle} from '{{ValueToParse}}'", text);
			return null;
		}

		private static Uri ParseUri(string uriText)
		{
			return uriText != null ? new Uri(uriText, UriKind.Absolute) : null;
		}

		private MovieRatingModel ParseRating(string ratingValueText, string ratingCountText)
		{
			var ratingValue = ParseDecimal(ratingValueText, "rating value");
			var ratingCount = ParseInt(ratingCountText, "rating count");

			if (ratingValue == null)
			{
				return null;
			}

			return new MovieRatingModel(ratingValue.Value, ratingCount);
		}

		private TimeSpan? ParseDuration(string durationText)
		{
			var duration = ParseInt(durationText, "duration");

			return duration != null ? TimeSpan.FromMinutes(duration.Value) : (TimeSpan?)null;
		}

		private static string SanitizeSummary(string summary)
		{
			if (String.IsNullOrWhiteSpace(summary))
			{
				return null;
			}

			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(summary);

			foreach (var node in htmlDoc.DocumentNode.SelectNodes("//br") ?? Enumerable.Empty<HtmlNode>())
			{
				node.ParentNode.ReplaceChild(htmlDoc.CreateTextNode("\n"), node);
			}

			return HtmlEntity.DeEntitize(htmlDoc.DocumentNode.InnerText);
		}
	}
}
