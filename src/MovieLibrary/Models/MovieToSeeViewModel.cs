using System;
using MovieLibrary.Logic.Models;
using static MovieLibrary.Models.Constants;

namespace MovieLibrary.Models
{
	public class MovieToSeeViewModel
	{
		public MovieId Id { get; }

		public DateTimeOffset TimestampOfAddingToSeeList { get; }

		public MovieInfoViewModel MovieInfo { get; }

		public string Reference { get; }

		public MovieToSeeViewModel(MovieToSeeModel movie)
		{
			_ = movie ?? throw new ArgumentNullException(nameof(movie));

			Id = movie.Id;
			TimestampOfAddingToSeeList = movie.TimestampOfAddingToSeeList;
			MovieInfo = new MovieInfoViewModel(movie.MovieInfo);
			Reference = String.IsNullOrEmpty(movie.Reference) ? MissingValue : movie.Reference;
		}
	}
}
