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
		public async Task<IActionResult> AddMovie(MoviesToSeeViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return await MoviesView(cancellationToken);
			}

			var newMovieToSee = model.NewMovieToSee;
			var newMovieId = await service.AddMovieByUrl(newMovieToSee.MovieUri, cancellationToken);

			return RedirectToAction("Index", "MoviesToSee", $"movie-{newMovieId.Value}");
		}

		[HttpGet]
		[Authorize(Roles = "CanMarkMoviesAsSeen")]
		public async Task<IActionResult> ConfirmMarkingAsSeen(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await service.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToSeeViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = "CanMarkMoviesAsSeen")]
		public async Task<IActionResult> MarkMovieAsSeen(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.MarkMovieAsSeen(movieId, cancellationToken);

			return RedirectToAction("Index");
		}

		[HttpPost]
		[Authorize(Roles = "CanMarkMoviesAsSeen")]
		public IActionResult CancelMarkingAsSeen()
		{
			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = "CanDeleteMoviesToSee")]
		public async Task<IActionResult> ConfirmMovieDeletion(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await service.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToSeeViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = "CanDeleteMoviesToSee")]
		public async Task<IActionResult> DeleteMovie([FromForm] string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.DeleteMovie(movieId, cancellationToken);

			return RedirectToAction("Index");
		}

		[HttpPost]
		[Authorize(Roles = "CanDeleteMoviesToSee")]
		public IActionResult CancelMovieDeletion()
		{
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
