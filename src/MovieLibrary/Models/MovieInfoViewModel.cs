using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MovieLibrary.Logic.Models;
using static System.FormattableString;
using static MovieLibrary.Models.Constants;

namespace MovieLibrary.Models
{
	public class MovieInfoViewModel
	{
		private const int MaxCollectionLength = 3;

		private readonly MovieInfoModel movieInfo;

		public string Title => movieInfo.Title;

		public string Year => movieInfo.Year != null ? movieInfo.Year.Value.ToString(CultureInfo.InvariantCulture) : MissingValue;

		public Uri MovieUri => movieInfo.MovieUri;

		public Uri PosterUri => movieInfo.PosterUri;

		public IReadOnlyCollection<string> Directors => GetSafeCollection(movieInfo.Directors);

		public IReadOnlyCollection<string> Cast => GetSafeCollection(movieInfo.Cast);

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

		public IReadOnlyCollection<string> Genres => GetSafeCollection(movieInfo.Genres);

		public IReadOnlyCollection<string> Countries => GetSafeCollection(movieInfo.Countries);

		public IReadOnlyCollection<string> SummaryParagraphs => movieInfo.SummaryParagraphs ?? Array.Empty<string>();

		public MovieInfoViewModel(MovieInfoModel movieInfo)
		{
			this.movieInfo = movieInfo ?? throw new ArgumentNullException(nameof(movieInfo));
		}

		private static List<string> GetSafeCollection(IReadOnlyCollection<string> source)
		{
			return source?.Take(MaxCollectionLength).ToList() ?? new List<string> { MissingValue };
		}
	}
}
