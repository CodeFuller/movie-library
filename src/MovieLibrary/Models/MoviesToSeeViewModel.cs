using System;
using System.Collections.Generic;
using System.Linq;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Models
{
	public class MoviesToSeeViewModel
	{
		public NewMovieViewModel NewMovieToSee { get; set; }

		public IReadOnlyCollection<MovieToSeeViewModel> Movies { get; }

		public bool AddedMovie { get; set; }

		public bool MarkedMovieAsSeen { get; set; }

		public bool DeletedMovie { get; set; }

		public MoviesToSeeViewModel()
		{
		}

		public MoviesToSeeViewModel(IEnumerable<MovieToSeeModel> movies)
		{
			Movies = movies?.Select(m => new MovieToSeeViewModel(m)).ToList() ?? throw new ArgumentNullException(nameof(movies));
		}
	}
}
