using System.Collections.Generic;

namespace MovieLibrary.Logic.Models
{
	public class MoviesToSeeModel
	{
		public IReadOnlyCollection<MovieToSeeModel> Movies { get; set; } = new List<MovieToSeeModel>();
	}
}
