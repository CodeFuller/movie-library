using System;
using MovieLibrary.Logic.Models;
using static MovieLibrary.Models.Constants;

namespace MovieLibrary.Models
{
	public class MovieToGetViewModel
	{
		public MovieId Id { get; }

		public DateTimeOffset TimestampOfAddingToGetList { get; }

		public MovieInfoViewModel MovieInfo { get; }

		public string Reference { get; }

		public MovieToGetViewModel(MovieToGetModel movie)
		{
			_ = movie ?? throw new ArgumentNullException(nameof(movie));

			Id = movie.Id;
			TimestampOfAddingToGetList = movie.TimestampOfAddingToGetList;
			MovieInfo = new MovieInfoViewModel(movie.MovieInfo);
			Reference = String.IsNullOrEmpty(movie.Reference) ? MissingValue : movie.Reference;
		}
	}
}
