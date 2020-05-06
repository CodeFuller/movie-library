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

		public bool AddedMovie { get; set; }

		public bool MovedMovie { get; set; }

		public bool DeletedMovie { get; set; }

		public PagingViewModel Paging { get; set; }

		public MoviesToGetViewModel()
		{
		}

		public MoviesToGetViewModel(IEnumerable<MovieToGetModel> movies, int currentPageNumber, int totalPagesNumber)
		{
			Movies = movies?.Select(m => new MovieToGetViewModel(m)).ToList() ?? throw new ArgumentNullException(nameof(movies));
			Paging = new PagingViewModel(currentPageNumber, totalPagesNumber);
		}
	}
}
