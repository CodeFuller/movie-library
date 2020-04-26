using System;

namespace MovieLibrary.Logic.Models
{
	public class MovieToSeeModel
	{
		public MovieId Id { get; }

		public DateTimeOffset TimestampOfAddingToSeeList { get; }

		public MovieInfoModel MovieInfo { get; }

		public MovieToSeeModel(MovieId id, DateTimeOffset timestampOfAdding, MovieInfoModel movieInfo)
		{
			Id = id ?? throw new ArgumentNullException(nameof(id));
			TimestampOfAddingToSeeList = timestampOfAdding;
			MovieInfo = movieInfo ?? throw new ArgumentNullException(nameof(movieInfo));
		}
	}
}
