using System;

namespace MovieLibrary.Logic.Models
{
	public class MovieToSeeModel
	{
		public MovieId Id { get; set; }

		public DateTimeOffset TimestampOfAddingToSeeList { get; set; }

		public MovieInfoModel MovieInfo { get; set; }

		public string Reference { get; set; }
	}
}
