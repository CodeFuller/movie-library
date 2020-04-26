using System;
using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Models
{
	public class MovieToGetModel
	{
		public MovieId Id { get; }

		public DateTimeOffset TimestampOfAddingToGetList { get; set; }

		public MovieInfoModel MovieInfo { get; }

		public MovieToGetModel(MovieId id, DateTimeOffset timestampOfAdding, MovieInfo movieInfo)
		{
			Id = id ?? throw new ArgumentNullException(nameof(id));
			TimestampOfAddingToGetList = timestampOfAdding;
			MovieInfo = new MovieInfoModel(movieInfo);
		}
	}
}
