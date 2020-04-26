using System;
using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Models
{
	public class MovieToSeeModel
	{
		public MovieId Id { get; }

		public DateTimeOffset TimestampOfAddingToSeeList { get; set; }

		public MovieInfoModel MovieInfo { get; }

		public MovieToSeeModel(MovieId id, DateTimeOffset timestampOfAdding, MovieInfo movieInfo)
		{
			Id = id ?? throw new ArgumentNullException(nameof(id));
			TimestampOfAddingToSeeList = timestampOfAdding;
			MovieInfo = new MovieInfoModel(movieInfo);
		}
	}
}
