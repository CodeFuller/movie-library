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
			return await MoviesView(cancellationToken);
		}

		[HttpPost]
		[Authorize(Roles = "MoviesToSeeAdder")]
		public async Task<IActionResult> Index(MoviesToSeeViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return await MoviesView(cancellationToken);
			}

			var newMovieToSee = model.NewMovieToSee;
			await service.AddMovieByUrl(newMovieToSee.MovieUri, cancellationToken);

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = "CanMarkMoviesAsSeen")]
		public async Task<IActionResult> MarkMovieAsSeen(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.MarkMovieAsSeen(movieId, cancellationToken);

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = "CanDeleteMoviesToSee")]
		public async Task<IActionResult> DeleteMovie(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.DeleteMovie(movieId, cancellationToken);

			return RedirectToAction("Index");
		}

		private async Task<IActionResult> MoviesView(CancellationToken cancellationToken)
		{
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
