using System;

namespace MovieLibrary.Logic.Models
{
	public class MovieToSeeModel
	{
		public MovieId Id { get; }

		public DateTimeOffset TimestampOfAddingToSeeList { get; }

		public MovieInfoModel MovieInfo { get; }

		public MovieToSeeModel(DateTimeOffset timestampOfAdding, MovieInfoModel movieInfo)
		{
			TimestampOfAddingToSeeList = timestampOfAdding;
			MovieInfo = movieInfo ?? throw new ArgumentNullException(nameof(movieInfo));
		}

		public MovieToSeeModel(MovieId id, DateTimeOffset timestampOfAdding, MovieInfoModel movieInfo)
			: this(timestampOfAdding, movieInfo)
		{
			Id = id ?? throw new ArgumentNullException(nameof(id));
		}
	}
}
