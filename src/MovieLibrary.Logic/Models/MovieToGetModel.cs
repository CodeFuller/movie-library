using System;

namespace MovieLibrary.Logic.Models
{
	public class MovieToGetModel
	{
		public MovieId Id { get; set; }

		public DateTimeOffset TimestampOfAddingToGetList { get; set; }

		public MovieInfoModel MovieInfo { get; set; }
	}
}
