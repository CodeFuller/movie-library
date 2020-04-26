using System;
using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Models
{
	public class MovieToSeeModel
	{
		public MovieId Id { get; }

		public MovieInfoModel MovieInfo { get; }

		public MovieToSeeModel(MovieId id, MovieInfo movieInfo)
		{
			Id = id ?? throw new ArgumentNullException(nameof(id));
			MovieInfo = new MovieInfoModel(movieInfo);
		}
	}
}
