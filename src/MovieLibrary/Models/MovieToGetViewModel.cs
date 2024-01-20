using System;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Models
{
	public class MovieToGetViewModel
	{
		public MovieId Id { get; }

		public DateTimeOffset TimestampOfAddingToGetList { get; }

		public MovieInfoViewModel MovieInfo { get; }

		public MovieToGetViewModel(MovieToGetModel movie)
		{
			_ = movie ?? throw new ArgumentNullException(nameof(movie));

			Id = movie.Id;
			TimestampOfAddingToGetList = movie.TimestampOfAddingToGetList;
			MovieInfo = new MovieInfoViewModel(movie.MovieInfo);
		}
	}
}
