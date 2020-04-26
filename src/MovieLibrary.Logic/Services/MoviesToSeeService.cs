using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Services
{
	internal class MoviesToSeeService : IMoviesToSeeService
	{
		private readonly IMoviesToSeeRepository repository;

		public MoviesToSeeService(IMoviesToSeeRepository repository)
		{
			this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		public IAsyncEnumerable<MovieToSeeModel> GetMoviesToSee(CancellationToken cancellationToken)
		{
			return repository
				.ReadMoviesToSee(cancellationToken)
				.Select(m => new MovieToSeeModel(m.Id, m.MovieInfo));
		}
	}
}
