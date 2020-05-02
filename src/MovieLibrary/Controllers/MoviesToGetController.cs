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
	public class MoviesToGetController : Controller
	{
		private readonly IMoviesToGetService service;

		public MoviesToGetController(IMoviesToGetService service)
		{
			this.service = service ?? throw new ArgumentNullException(nameof(service));
		}

		[HttpGet]
		[Authorize(Roles = "MoviesToGetReader")]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			return await MoviesView(cancellationToken);
		}

		[HttpPost]
		[Authorize(Roles = "MoviesToGetAdder")]
		public async Task<IActionResult> AddMovie(MoviesToGetViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return await MoviesView(cancellationToken);
			}

			var newMovieToGet = model.NewMovieToGet;
			var newMovieId = await service.AddMovieByUrl(newMovieToGet.MovieUri, cancellationToken);

			return RedirectToAction("Index", "MoviesToGet", $"movie-{newMovieId.Value}");
		}

		[HttpGet]
		[Authorize(Roles = "MoviesToSeeAdder")]
		public async Task<IActionResult> MoveToMoviesToSee(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.MoveToMoviesToSee(movieId, cancellationToken);

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = "CanDeleteMoviesToGet")]
		public async Task<IActionResult> ConfirmMovieDeletion(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await service.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToGetViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = "CanDeleteMoviesToGet")]
		public async Task<IActionResult> DeleteMovie([FromForm] string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.DeleteMovie(movieId, cancellationToken);

			return RedirectToAction("Index");
		}

		[HttpPost]
		[Authorize(Roles = "CanDeleteMoviesToGet")]
		public IActionResult CancelMovieDeletion()
		{
			return RedirectToAction("Index");
		}

		private async Task<IActionResult> MoviesView(CancellationToken cancellationToken)
		{
			var viewModel = await ReadMoviesToGet(cancellationToken);

			return View("Index", viewModel);
		}

		private async Task<MoviesToGetViewModel> ReadMoviesToGet(CancellationToken cancellationToken)
		{
			var moviesToGet = await service.GetAllMovies(cancellationToken).ToListAsync(cancellationToken);

			return new MoviesToGetViewModel(moviesToGet);
		}
	}
}
