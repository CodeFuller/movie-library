using System;
using System.Collections.Generic;
using System.Globalization;
using MovieLibrary.Logic.MoviesInfo;
using static System.FormattableString;

namespace MovieLibrary.Logic.Models
{
	public class MovieInfoModel
	{
		private const string MissingValue = "N/A";

		private readonly MovieInfo movieInfo;

		public string Title => movieInfo.Title;

		public string Year => movieInfo.Year != null ? movieInfo.Year.Value.ToString(CultureInfo.InvariantCulture) : MissingValue;

		public Uri MovieUri => movieInfo.MovieUri;

		public Uri PosterUri => movieInfo.PosterUri;

		public IReadOnlyCollection<string> Directors => GetSafeCollection(movieInfo.Directors);

		public IReadOnlyCollection<string> Cast => GetSafeCollection(movieInfo.Directors);

		public string Rating
		{
			get
			{
				var rating = movieInfo.Rating;

				if (rating == null)
				{
					return MissingValue;
				}

				var votesNumberPart = rating.VotesNumber == null ? String.Empty : Invariant($" ({rating.VotesNumber:N0})");
				return Invariant($"{rating.Value:N1}{votesNumberPart}");
			}
		}

		public string Duration => movieInfo.Duration == null ? MissingValue : Invariant($"{movieInfo.Duration.Value.ToString("hh\\:mm", CultureInfo.InvariantCulture)}");

		public IReadOnlyCollection<string> Genres => GetSafeCollection(movieInfo.Directors);

		public string Summary => movieInfo.Summary ?? String.Empty;

		public MovieInfoModel(MovieInfo movieInfo)
		{
			this.movieInfo = movieInfo ?? throw new ArgumentNullException(nameof(movieInfo));
		}

		private static IReadOnlyCollection<string> GetSafeCollection(IReadOnlyCollection<string> source)
		{
			return source ?? new[] { MissingValue };
		}
	}
}
