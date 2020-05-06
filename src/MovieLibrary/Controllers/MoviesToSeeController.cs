using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieLibrary.Internal;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using MovieLibrary.Models;

namespace MovieLibrary.Controllers
{
	public class MoviesToSeeController : Controller
	{
		private const string TempDataAddedMovie = "AddedMovie";
		private const string TempDataMarkedMovieAsSeen = "MarkedMovieAsSeen";
		private const string TempDataDeletedMovie = "DeletedMovie";

		private readonly IMoviesToSeeService service;

		private readonly AppSettings settings;

		public MoviesToSeeController(IMoviesToSeeService service, IOptions<AppSettings> options)
		{
			this.service = service ?? throw new ArgumentNullException(nameof(service));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanAddOrReadMoviesToSee)]
		public async Task<IActionResult> Index([FromRoute] int pageNumber, CancellationToken cancellationToken)
		{
			return await MoviesView(pageNumber, cancellationToken);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanAddMoviesToSee)]
		public async Task<IActionResult> AddMovie(MoviesToSeeViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return await MoviesView(model?.Paging?.CurrentPageNumber ?? 1, cancellationToken);
			}

			var newMovieToSee = model.NewMovieToSee;
			var newMovieId = await service.AddMovieByUrl(newMovieToSee.MovieUri, cancellationToken);

			TempData[TempDataAddedMovie] = true;

			return RedirectToAction("Index", "MoviesToSee", $"movie-{newMovieId.Value}");
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanMarkMoviesAsSeen)]
		public async Task<IActionResult> ConfirmMarkingAsSeen(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await service.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToSeeViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanMarkMoviesAsSeen)]
		public async Task<IActionResult> MarkMovieAsSeen(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.MarkMovieAsSeen(movieId, cancellationToken);

			TempData[TempDataMarkedMovieAsSeen] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanDeleteMoviesToSee)]
		public async Task<IActionResult> ConfirmMovieDeletion(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await service.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToSeeViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanDeleteMoviesToSee)]
		public async Task<IActionResult> DeleteMovie([FromForm] string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.DeleteMovie(movieId, cancellationToken);

			TempData[TempDataDeletedMovie] = true;

			return RedirectToAction("Index");
		}

		private async Task<IActionResult> MoviesView(int pageNumber, CancellationToken cancellationToken)
		{
			var viewModel = await ReadMoviesToSee(pageNumber, cancellationToken);

			viewModel.AddedMovie = TempData.GetBooleanValue(TempDataAddedMovie);
			viewModel.MarkedMovieAsSeen = TempData.GetBooleanValue(TempDataMarkedMovieAsSeen);
			viewModel.DeletedMovie = TempData.GetBooleanValue(TempDataDeletedMovie);

			return View("Index", viewModel);
		}

		private async Task<MoviesToSeeViewModel> ReadMoviesToSee(int pageNumber, CancellationToken cancellationToken)
		{
			var movies = await service.GetAllMovies(cancellationToken).ToListAsync(cancellationToken);

			var moviesCount = movies.Count;

			var pageMovies = movies
				.Skip((pageNumber - 1) * settings.MoviesPageSize)
				.Take(settings.MoviesPageSize);

			var totalPagesNumber = (int)Math.Ceiling(moviesCount / (double)settings.MoviesPageSize);

			return new MoviesToSeeViewModel(pageMovies, pageNumber, totalPagesNumber);
		}
	}
}
