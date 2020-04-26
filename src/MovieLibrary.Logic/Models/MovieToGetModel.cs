using System;

namespace MovieLibrary.Logic.Models
{
	public class MovieToGetModel
	{
		public MovieId Id { get; }

		public DateTimeOffset TimestampOfAddingToGetList { get; }

		public MovieInfoModel MovieInfo { get; }

		public MovieToGetModel(DateTimeOffset timestampOfAdding, MovieInfoModel movieInfo)
		{
			TimestampOfAddingToGetList = timestampOfAdding;
			MovieInfo = movieInfo ?? throw new ArgumentNullException(nameof(movieInfo));
		}

		public MovieToGetModel(MovieId id, DateTimeOffset timestampOfAdding, MovieInfoModel movieInfo)
			: this(timestampOfAdding, movieInfo)
		{
			Id = id ?? throw new ArgumentNullException(nameof(id));
		}
	}
}
