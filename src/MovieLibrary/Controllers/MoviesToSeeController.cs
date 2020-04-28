using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using MovieLibrary.Models;

namespace MovieLibrary.Controllers
{
	public class MoviesToSeeController : Controller
	{
		private readonly IMoviesToSeeService service;

		public MoviesToSeeController(IMoviesToSeeService service)
		{
			this.service = service ?? throw new ArgumentNullException(nameof(service));
		}

		[HttpGet]
		[Authorize(Roles = "MoviesToSeeReader")]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			var viewModel = await ReadMoviesToSee(cancellationToken);

			return View(viewModel);
		}

		[HttpGet]
		[Authorize(Roles = "CanMarkMoviesAsSeen")]
		public async Task<IActionResult> MarkMovieAsSeen(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.MarkMovieAsSeen(movieId, cancellationToken);

			var viewModel = await ReadMoviesToSee(cancellationToken);

			return View("Index", viewModel);
		}

		private async Task<MoviesToSeeViewModel> ReadMoviesToSee(CancellationToken cancellationToken)
		{
			var movies = await service.GetAllMovies(cancellationToken).ToListAsync(cancellationToken);

			return new MoviesToSeeViewModel(movies);
		}
	}
}
