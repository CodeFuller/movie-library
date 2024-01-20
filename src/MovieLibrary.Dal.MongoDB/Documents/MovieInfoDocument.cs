using System;
using System.Collections.Generic;

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

		public IReadOnlyCollection<string> SummaryParagraphs { get; set; }
	}
}
