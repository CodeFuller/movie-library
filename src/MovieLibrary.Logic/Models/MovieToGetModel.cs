using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Models
{
	public class MovieToGetModel
	{
		public MovieId Id { get; set; }

		public MovieInfo MovieInfo { get; set; }
	}
}
