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

		private readonly IMoviesToSeeService moviesToSeeService;

		private readonly IMovieInfoService movieInfoService;

		private readonly AppSettings settings;

		public MoviesToSeeController(IMoviesToSeeService moviesToSeeService, IMovieInfoService movieInfoService, IOptions<AppSettings> options)
		{
			this.moviesToSeeService = moviesToSeeService ?? throw new ArgumentNullException(nameof(moviesToSeeService));
			this.movieInfoService = movieInfoService ?? throw new ArgumentNullException(nameof(movieInfoService));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanAddOrReadMoviesToSee)]
		public IActionResult Index([FromRoute] int pageNumber)
		{
			return MoviesPageView(pageNumber);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanAddMoviesToSee)]
		public async Task<IActionResult> ConfirmMovieAdding(MoviesToSeeViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return MoviesPageView(model?.Paging?.CurrentPageNumber ?? 1);
			}

			var newMovieToSee = model.NewMovieToSee;
			var movieInfo = await movieInfoService.LoadMovieInfoByUrl(newMovieToSee.MovieUri, cancellationToken);

			return View("ConfirmMovieAdding", new InputMovieInfoViewModel(movieInfo));
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanAddMoviesToSee)]
		public async Task<IActionResult> AddMovie(InputMovieInfoViewModel model, CancellationToken cancellationToken)
		{
			var movieInfo = model.ToMovieInfo();
			await moviesToSeeService.AddMovie(movieInfo, cancellationToken);

			TempData[TempDataAddedMovie] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanMarkMoviesAsSeen)]
		public async Task<IActionResult> ConfirmMarkingAsSeen(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await moviesToSeeService.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToSeeViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanMarkMoviesAsSeen)]
		public async Task<IActionResult> MarkMovieAsSeen(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await moviesToSeeService.MarkMovieAsSeen(movieId, cancellationToken);

			TempData[TempDataMarkedMovieAsSeen] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanDeleteMoviesToSee)]
		public async Task<IActionResult> ConfirmMovieDeletion(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await moviesToSeeService.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToSeeViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanDeleteMoviesToSee)]
		public async Task<IActionResult> DeleteMovie([FromForm] string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await moviesToSeeService.DeleteMovie(movieId, cancellationToken);

			TempData[TempDataDeletedMovie] = true;

			return RedirectToAction("Index");
		}

		private IActionResult MoviesPageView(int pageNumber)
		{
			var moviesQueryable = moviesToSeeService.GetAllMovies();

			var moviesCount = moviesQueryable.Count();
			var totalPagesNumber = (int)Math.Ceiling(moviesCount / (double)settings.MoviesPageSize);

			if (pageNumber < 1)
			{
				return RedirectToAction("Index", new { pageNumber = 1 });
			}

			if (pageNumber > totalPagesNumber)
			{
				return RedirectToAction("Index", new { pageNumber = totalPagesNumber });
			}

			return CreateMoviesPageView(moviesQueryable, pageNumber, totalPagesNumber);
		}

		private ViewResult CreateMoviesPageView(IQueryable<MovieToSeeModel> movies, int pageNumber, int totalPagesNumber)
		{
			var pageMovies = movies
				.Skip((pageNumber - 1) * settings.MoviesPageSize)
				.Take(settings.MoviesPageSize);

			var viewModel = new MoviesToSeeViewModel(pageMovies, pageNumber, totalPagesNumber)
			{
				AddedMovie = TempData.GetBooleanValue(TempDataAddedMovie),
				MarkedMovieAsSeen = TempData.GetBooleanValue(TempDataMarkedMovieAsSeen),
				DeletedMovie = TempData.GetBooleanValue(TempDataDeletedMovie),
			};

			return View("Index", viewModel);
		}
	}
}
