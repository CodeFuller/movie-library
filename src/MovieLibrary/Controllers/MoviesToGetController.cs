using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieLibrary.Authorization;
using MovieLibrary.Internal;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using MovieLibrary.Models;

namespace MovieLibrary.Controllers
{
	public class MoviesToGetController : BasicMovieController<MovieToGetModel, MoviesToGetViewModel>
	{
		private const string TempDataErrorMessage = "Error";
		private const string TempDataAddedMovie = "AddedMovie";
		private const string TempDataMovedMovie = "MovedMovie";
		private const string TempDataDeletedMovie = "DeletedMovie";

		private readonly IMoviesToGetService moviesToGetService;

		private readonly IMovieUniquenessChecker movieUniquenessChecker;

		private readonly IMovieInfoService movieInfoService;

		protected override string ControllerName => "MoviesToGet";

		public MoviesToGetController(IMoviesToGetService moviesToGetService, IMovieUniquenessChecker movieUniquenessChecker, IMovieInfoService movieInfoService, IOptions<AppSettings> options)
			: base(options)
		{
			this.moviesToGetService = moviesToGetService ?? throw new ArgumentNullException(nameof(moviesToGetService));
			this.movieUniquenessChecker = movieUniquenessChecker ?? throw new ArgumentNullException(nameof(movieUniquenessChecker));
			this.movieInfoService = movieInfoService ?? throw new ArgumentNullException(nameof(movieInfoService));
		}

		[HttpGet]
		[Authorize(ApplicationPermissions.MoviesToGet.AddOrRead)]
		public IActionResult Index([FromRoute] int pageNumber)
		{
			return MoviesPageView(pageNumber);
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToGet.Add)]
		public async Task<IActionResult> ConfirmMovieAdding([FromForm] MoviesToGetViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return MoviesPageView(model?.Paging?.CurrentPageNumber ?? 1);
			}

			var newMovieToGet = model.NewMovieToGet;
			var movieUri = newMovieToGet.MovieUri;

			var checkResult = await movieUniquenessChecker.CheckMovie(movieUri, cancellationToken);
			if (checkResult != MovieUniquenessCheckResult.MovieIsUnique)
			{
				// Clearing movie URL from the input.
				ModelState.Clear();

				FillDuplicatedMovieError(checkResult, movieUri);

				return MoviesPageView(model.Paging?.CurrentPageNumber ?? 1);
			}

			var movieInfo = await movieInfoService.LoadMovieInfoByUrl(movieUri, cancellationToken);

			return View("ConfirmMovieAdding", new InputMovieInfoViewModel(movieInfo, newMovieToGet.Reference));
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToGet.Add)]
		public async Task<RedirectToActionResult> AddMovie([FromForm] InputMovieInfoViewModel model, CancellationToken cancellationToken)
		{
			var movieInfo = model.ToMovieInfo();

			var checkResult = await movieUniquenessChecker.CheckMovie(movieInfo.MovieUri, cancellationToken);
			if (checkResult != MovieUniquenessCheckResult.MovieIsUnique)
			{
				FillDuplicatedMovieError(checkResult, movieInfo.MovieUri);

				return RedirectToAction("Index");
			}

			await moviesToGetService.AddMovie(movieInfo, model.Reference, cancellationToken);

			TempData[TempDataAddedMovie] = true;

			return RedirectToAction("Index");
		}

		private void FillDuplicatedMovieError(MovieUniquenessCheckResult checkResult, Uri movieUri)
		{
			TempData[TempDataErrorMessage] = GetDuplicatedMovieError(checkResult, movieUri);
		}

		[HttpGet]
		[Authorize(ApplicationPermissions.MoviesToGet.MoveToMoviesToSee)]
		public async Task<ViewResult> ConfirmMovingToSee([FromRoute] string id, CancellationToken cancellationToken)
		{
			var viewModel = await CreateMovieViewModel(id, cancellationToken);
			return View(viewModel);
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToGet.MoveToMoviesToSee)]
		public async Task<RedirectToActionResult> MoveToMoviesToSee([FromForm] string id, CancellationToken cancellationToken)
		{
			var movieId = CreateMovieId(id);
			await moviesToGetService.MoveToMoviesToSee(movieId, cancellationToken);

			TempData[TempDataMovedMovie] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(ApplicationPermissions.MoviesToGet.Delete)]
		public async Task<ViewResult> ConfirmMovieDeletion([FromRoute] string id, CancellationToken cancellationToken)
		{
			var viewModel = await CreateMovieViewModel(id, cancellationToken);
			return View(viewModel);
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToGet.Delete)]
		public async Task<RedirectToActionResult> DeleteMovie([FromForm] string id, CancellationToken cancellationToken)
		{
			var movieId = CreateMovieId(id);
			await moviesToGetService.DeleteMovie(movieId, cancellationToken);

			TempData[TempDataDeletedMovie] = true;

			return RedirectToAction("Index");
		}

		protected override IQueryable<MovieToGetModel> GetAllMovies()
		{
			return moviesToGetService.GetAllMovies();
		}

		protected override MoviesToGetViewModel CreateMoviesPageViewModel(IEnumerable<MovieToGetModel> movies, int pageNumber, int totalPagesNumber)
		{
			return new(movies, pageNumber, totalPagesNumber)
			{
				ErrorMessage = TempData.GetStringValue(TempDataErrorMessage),
				AddedMovie = TempData.GetBooleanValue(TempDataAddedMovie),
				MovedMovie = TempData.GetBooleanValue(TempDataMovedMovie),
				DeletedMovie = TempData.GetBooleanValue(TempDataDeletedMovie),
			};
		}

		private async Task<MovieToGetViewModel> CreateMovieViewModel(string id, CancellationToken cancellationToken)
		{
			var movieId = CreateMovieId(id);
			var movie = await moviesToGetService.GetMovie(movieId, cancellationToken);
			return new MovieToGetViewModel(movie);
		}
	}
}
