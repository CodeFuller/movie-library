using System;
using System.Collections.Generic;
using System.Linq;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Models
{
	public class MoviesToGetViewModel
	{
		public NewMovieViewModel NewMovieToGet { get; set; }

		public IReadOnlyCollection<MovieToGetViewModel> Movies { get; }

		public MoviesToGetViewModel()
		{
		}

		public MoviesToGetViewModel(IEnumerable<MovieToGetModel> movies)
		{
			Movies = movies?.Select(m => new MovieToGetViewModel(m)).ToList() ?? throw new ArgumentNullException(nameof(movies));
		}
	}
}
