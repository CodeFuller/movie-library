using System.Collections.Generic;

namespace MovieLibrary.Logic.Models
{
	public class MoviesToGetModel
	{
		public NewMovieToGetModel NewMovieToGet { get; set; }

		public IReadOnlyCollection<MovieToGetModel> Movies { get; set; } = new List<MovieToGetModel>();
	}
}
