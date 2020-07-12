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
		private static readonly Regex TitleRegex = new Regex(@"<h1 class=""styles_title_[^>]+? itemProp=""name""><span class=""styles_title_[^>]+?>([^<>]+?)</span>", RegexOptions.Compiled);
		private static readonly Regex YearRegex = new Regex(@"<a [^>]*?href=""/lists/navigator/(\d+)/\?quick_filters=films"">\1</a>", RegexOptions.Compiled);
		private static readonly Regex MovieUriRegex = new Regex(@"<a class=""styles[^>]+?"" href=""(/film/\d+/)subscribe/"">", RegexOptions.Compiled);
		private static readonly Regex PosterUriRegex = new Regex(@"<img class=""film-poster [^>]+? src=""(//avatars.mds.yandex.net/get-kinopoisk-image/[^>]+?)""", RegexOptions.Compiled);
		private static readonly Regex DirectorsRegex = new Regex(@"<div class=""styles_title[^>]+?"">Режиссер</div><div[^<>]*?>(?:<a class=""styles[^>]+?"" href=""/name/\d+/""[^>]*?>([^<>]+?)</a>(?:,\s*)?)+(?:, <a href=""/film/\d+/cast/who_is/director/""[^<>]*?>\.\.\.</a>)?</div></div>", RegexOptions.Compiled);
		private static readonly Regex CastRegex = new Regex(@"<a href=""/film/\d+/cast/"" class=""styles[^>]+?""[^>]*?>В главных ролях</a></h3><ul[^>]*?>(?:<li[^>]*?><a href=""/name/\d+/""[^>]*?itemProp=""actor"">([^<>]+?)</a></li>)+", RegexOptions.Compiled);
		private static readonly Regex RatingValueRegex = new Regex(@"<a class=""film-rating-value[^<>]*?>(\d+\.\d+)</a></span><span[^<>]*?>\d+(?: \d+)*</span></div>", RegexOptions.Compiled);
		private static readonly Regex RatingCountRegex = new Regex(@"<a class=""film-rating-value[^<>]*?>\d+\.\d+</a></span><span[^<>]*?>(\d+(?: \d+)*)</span></div>", RegexOptions.Compiled);
		private static readonly Regex DurationRegex = new Regex(@"<div[^<>]*?>Время</div><div[^<>]*?><div[^<>]*?>(\d+) мин\.", RegexOptions.Compiled);
		private static readonly Regex GenresRegex = new Regex(@"<div[^<>]*?>Жанр</div><div[^<>]*?><div[^<>]*?>(?:<a[^<>]*?href=""/lists/navigator/.+?/\?quick_filters=films""[^<>]*?>([^<>]+?)</a>(?:, )?)+", RegexOptions.Compiled);
		private static readonly Regex SummaryRegex = new Regex(@"<div[^<>]*?><div[^<>]*?>(?:<p[^<>]*?>(.+?)</p>)+</div></div>", RegexOptions.Compiled);

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

			List<string> summaryParagraphs = null;

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

				summaryParagraphs ??= ParseMultipleValues(line, SummaryRegex);
			}

			if (String.Equals(posterUri, "https://st.kp.yandex.net/images/movies/poster_none.png", StringComparison.Ordinal))
			{
				posterUri = null;
			}

			var movieInfo = new MovieInfoModel
			{
				Title = SanitizeText(title),
				Year = ParseInt(yearText, "year"),
				MovieUri = movieUri != null ? new Uri(new Uri("https://www.kinopoisk.ru"), movieUri) : null,
				PosterUri = posterUri != null ? new Uri($"https:{posterUri}", UriKind.Absolute) : null,
				Directors = SanitizeText(directors),
				Cast = SanitizeText(cast),
				Rating = ParseRating(ratingValue, ratingCount),
				Duration = ParseDuration(duration),
				Genres = SanitizeText(genres),
				Summary = summaryParagraphs != null ? String.Join("\n\n", summaryParagraphs.Select(SanitizeText)) : null,
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

		private MovieRatingModel ParseRating(string ratingValueText, string ratingCountText)
		{
			var ratingValue = ParseDecimal(ratingValueText, "rating value");

			ratingCountText = ratingCountText?.Replace(" ", String.Empty, StringComparison.Ordinal);
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

		private static IReadOnlyCollection<string> SanitizeText(IReadOnlyCollection<string> summary)
		{
			return summary?.Select(SanitizeText).ToList();
		}

		private static string SanitizeText(string summary)
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
