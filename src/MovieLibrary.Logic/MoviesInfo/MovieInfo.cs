using System;
using System.Collections.Generic;

namespace MovieLibrary.Logic.MoviesInfo
{
	public class MovieInfo
	{
		public string Title { get; set; }

		public Uri MovieUri { get; set; }

		public Uri PosterUri { get; set; }

		public IReadOnlyCollection<string> Genres { get; set; }
	}
}
