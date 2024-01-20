using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieLibrary.Dal.MongoDB.Documents
{
	internal class MovieInfoDocument
	{
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
			if (SummaryParagraphs?.Any() == true)
			{
				return SummaryParagraphs;
			}

			return Summary?.Split(new[] { "\r\n\r\n", "\n\n", "\n\r" }, StringSplitOptions.None);
		}
	}
}
