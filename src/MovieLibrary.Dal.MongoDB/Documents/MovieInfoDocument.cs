using System;
using System.Collections.Generic;

namespace MovieLibrary.Dal.MongoDB.Documents
{
	internal class MovieInfoDocument
	{
		private static readonly string[] NewLineSeparators = { "\r\n\r\n", "\n\n", "\n\r" };

		public string Title { get; set; }

		public int? Year { get; set; }

		public Uri MovieUri { get; set; }

		public Uri PosterUri { get; set; }

		public IReadOnlyCollection<string> Directors { get; set; }

		public IReadOnlyCollection<string> Cast { get; set; }

		public decimal? RatingValue { get; set; }

		public int? RatingVotesNumber { get; set; }

		public TimeSpan? Duration { get; set; }

		public IReadOnlyCollection<string> Genres { get; set; }

		public IReadOnlyCollection<string> Countries { get; set; }

		// TODO: Remove this property when there is no more old movies in the database.
		[Obsolete($"Use {nameof(SummaryParagraphs)} property instead")]
		public string Summary { get; set; }

		public IReadOnlyCollection<string> SummaryParagraphs { get; set; }

		public IReadOnlyCollection<string> GetSummaryParagraphs()
		{
			if (SummaryParagraphs?.Count > 0)
			{
				return SummaryParagraphs;
			}

#pragma warning disable CS0618 // Type or member is obsolete
			return Summary?.Split(NewLineSeparators, StringSplitOptions.None);
#pragma warning restore CS0618 // Type or member is obsolete
		}
	}
}
