using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Internal;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using MovieLibrary.Models;

namespace MovieLibrary.Controllers
{
	public class MoviesToGetController : Controller
	{
		private const string TempDataAddedMovie = "AddedMovie";
		private const string TempDataMovedMovie = "MovedMovie";
		private const string TempDataDeletedMovie = "DeletedMovie";

		private readonly IMoviesToGetService service;

		public MoviesToGetController(IMoviesToGetService service)
		{
			this.service = service ?? throw new ArgumentNullException(nameof(service));
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanAddOrReadMoviesToGet)]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			return await MoviesView(cancellationToken);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanAddMoviesToGet)]
		public async Task<IActionResult> AddMovie(MoviesToGetViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return await MoviesView(cancellationToken);
			}

			var newMovieToGet = model.NewMovieToGet;
			var newMovieId = await service.AddMovieByUrl(newMovieToGet.MovieUri, cancellationToken);

			TempData[TempDataAddedMovie] = true;

			return RedirectToAction("Index", "MoviesToGet", $"movie-{newMovieId.Value}");
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanAddMoviesToSee)]
		public async Task<IActionResult> ConfirmMovingToSee(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await service.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToGetViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanAddMoviesToSee)]
		public async Task<IActionResult> MoveToMoviesToSee(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.MoveToMoviesToSee(movieId, cancellationToken);

			TempData[TempDataMovedMovie] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanDeleteMoviesToGet)]
		public async Task<IActionResult> ConfirmMovieDeletion(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await service.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToGetViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanDeleteMoviesToGet)]
		public async Task<IActionResult> DeleteMovie([FromForm] string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.DeleteMovie(movieId, cancellationToken);

			TempData[TempDataDeletedMovie] = true;

			return RedirectToAction("Index");
		}

		private async Task<IActionResult> MoviesView(CancellationToken cancellationToken)
		{
			var viewModel = await ReadMoviesToGet(cancellationToken);
			viewModel.AddedMovie = TempData.GetBooleanValue(TempDataAddedMovie);
			viewModel.MovedMovie = TempData.GetBooleanValue(TempDataMovedMovie);
			viewModel.DeletedMovie = TempData.GetBooleanValue(TempDataDeletedMovie);

			return View("Index", viewModel);
		}

		private async Task<MoviesToGetViewModel> ReadMoviesToGet(CancellationToken cancellationToken)
		{
			var moviesToGet = await service.GetAllMovies(cancellationToken).ToListAsync(cancellationToken);

			return new MoviesToGetViewModel(moviesToGet);
		}
	}
}
