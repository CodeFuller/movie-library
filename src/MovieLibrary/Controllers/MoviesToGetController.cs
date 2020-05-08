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
	public class MoviesToGetController : Controller
	{
		private const string TempDataAddedMovie = "AddedMovie";
		private const string TempDataMovedMovie = "MovedMovie";
		private const string TempDataDeletedMovie = "DeletedMovie";

		private readonly IMoviesToGetService moviesToGetService;

		private readonly IMovieInfoService movieInfoService;

		private readonly AppSettings settings;

		public MoviesToGetController(IMoviesToGetService moviesToGetService, IMovieInfoService movieInfoService, IOptions<AppSettings> options)
		{
			this.moviesToGetService = moviesToGetService ?? throw new ArgumentNullException(nameof(moviesToGetService));
			this.movieInfoService = movieInfoService ?? throw new ArgumentNullException(nameof(movieInfoService));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanAddOrReadMoviesToGet)]
		public IActionResult Index([FromRoute] int pageNumber)
		{
			return MoviesPageView(pageNumber);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanAddMoviesToGet)]
		public async Task<IActionResult> ConfirmMovieAdding(MoviesToGetViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return MoviesPageView(model?.Paging?.CurrentPageNumber ?? 1);
			}

			var newMovieToGet = model.NewMovieToGet;
			var movieInfo = await movieInfoService.LoadMovieInfoByUrl(newMovieToGet.MovieUri, cancellationToken);

			return View("ConfirmMovieAdding", new InputMovieInfoViewModel(movieInfo));
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanAddMoviesToGet)]
		public async Task<IActionResult> AddMovie(InputMovieInfoViewModel model, CancellationToken cancellationToken)
		{
			var movieInfo = model.ToMovieInfo();
			await moviesToGetService.AddMovie(movieInfo, cancellationToken);

			TempData[TempDataAddedMovie] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanAddMoviesToSee)]
		public async Task<IActionResult> ConfirmMovingToSee(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await moviesToGetService.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToGetViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanAddMoviesToSee)]
		public async Task<IActionResult> MoveToMoviesToSee(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await moviesToGetService.MoveToMoviesToSee(movieId, cancellationToken);

			TempData[TempDataMovedMovie] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = Roles.CanDeleteMoviesToGet)]
		public async Task<IActionResult> ConfirmMovieDeletion(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			var movie = await moviesToGetService.GetMovie(movieId, cancellationToken);
			var viewModel = new MovieToGetViewModel(movie);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = Roles.CanDeleteMoviesToGet)]
		public async Task<IActionResult> DeleteMovie([FromForm] string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await moviesToGetService.DeleteMovie(movieId, cancellationToken);

			TempData[TempDataDeletedMovie] = true;

			return RedirectToAction("Index");
		}

		private IActionResult MoviesPageView(int pageNumber)
		{
			var moviesQueryable = moviesToGetService.GetAllMovies();

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

		private ViewResult CreateMoviesPageView(IQueryable<MovieToGetModel> movies, int pageNumber, int totalPagesNumber)
		{
			var pageMovies = movies
				.Skip((pageNumber - 1) * settings.MoviesPageSize)
				.Take(settings.MoviesPageSize);

			var viewModel = new MoviesToGetViewModel(pageMovies, pageNumber, totalPagesNumber)
			{
				AddedMovie = TempData.GetBooleanValue(TempDataAddedMovie),
				MovedMovie = TempData.GetBooleanValue(TempDataMovedMovie),
				DeletedMovie = TempData.GetBooleanValue(TempDataDeletedMovie),
			};

			return View("Index", viewModel);
		}
	}
}
