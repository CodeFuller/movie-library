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
	public class MoviesToSeeController : BasicMovieController<MovieToSeeModel, MoviesToSeeViewModel>
	{
		private const string TempDataErrorMessage = "Error";
		private const string TempDataAddedMovie = "AddedMovie";
		private const string TempDataMarkedMovieAsSeen = "MarkedMovieAsSeen";
		private const string TempDataDeletedMovie = "DeletedMovie";

		private readonly IMoviesToSeeService moviesToSeeService;

		private readonly IMovieUniquenessChecker movieUniquenessChecker;

		private readonly IMovieInfoService movieInfoService;

		protected override string ControllerName => "MoviesToSee";

		public MoviesToSeeController(IMoviesToSeeService moviesToSeeService, IMovieUniquenessChecker movieUniquenessChecker, IMovieInfoService movieInfoService, IOptions<AppSettings> options)
			: base(options)
		{
			this.moviesToSeeService = moviesToSeeService ?? throw new ArgumentNullException(nameof(moviesToSeeService));
			this.movieUniquenessChecker = movieUniquenessChecker ?? throw new ArgumentNullException(nameof(movieUniquenessChecker));
			this.movieInfoService = movieInfoService ?? throw new ArgumentNullException(nameof(movieInfoService));
		}

		[HttpGet]
		[Authorize(ApplicationPermissions.MoviesToSee.AddOrRead)]
		public IActionResult Index([FromRoute] int pageNumber)
		{
			return MoviesPageView(pageNumber);
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToSee.Add)]
		public async Task<IActionResult> ConfirmMovieAdding([FromForm] MoviesToSeeViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return MoviesPageView(model?.Paging?.CurrentPageNumber ?? 1);
			}

			var newMovieToSee = model.NewMovieToSee;
			var movieUri = newMovieToSee.MovieUri;

			var checkResult = await movieUniquenessChecker.CheckMovie(movieUri, cancellationToken);
			if (checkResult != MovieUniquenessCheckResult.MovieIsUnique)
			{
				// Clearing movie URL from the input.
				ModelState.Clear();

				FillDuplicatedMovieError(checkResult, movieUri);

				return MoviesPageView(model.Paging?.CurrentPageNumber ?? 1);
			}

			var movieInfo = await movieInfoService.LoadMovieInfoByUrl(movieUri, cancellationToken);

			return View("ConfirmMovieAdding", new InputMovieInfoViewModel(movieInfo, newMovieToSee.Reference));
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToSee.Add)]
		public async Task<IActionResult> AddMovie([FromForm] InputMovieInfoViewModel model, CancellationToken cancellationToken)
		{
			var movieInfo = model.ToMovieInfo();

			var checkResult = await movieUniquenessChecker.CheckMovie(movieInfo.MovieUri, cancellationToken);
			if (checkResult != MovieUniquenessCheckResult.MovieIsUnique)
			{
				FillDuplicatedMovieError(checkResult, movieInfo.MovieUri);

				return RedirectToAction("Index");
			}

			await moviesToSeeService.AddMovie(movieInfo, model.Reference, cancellationToken);

			TempData[TempDataAddedMovie] = true;

			return RedirectToAction("Index");
		}

		private void FillDuplicatedMovieError(MovieUniquenessCheckResult checkResult, Uri movieUri)
		{
			TempData[TempDataErrorMessage] = GetDuplicatedMovieError(checkResult, movieUri);
		}

		[HttpGet]
		[Authorize(ApplicationPermissions.MoviesToSee.MarkAsSeen)]
		public async Task<ViewResult> ConfirmMarkingAsSeen([FromRoute] string id, CancellationToken cancellationToken)
		{
			var viewModel = await CreateMovieViewModel(id, cancellationToken);
			return View(viewModel);
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToSee.MarkAsSeen)]
		public async Task<RedirectToActionResult> MarkMovieAsSeen([FromForm] string id, CancellationToken cancellationToken)
		{
			var movieId = CreateMovieId(id);
			await moviesToSeeService.MarkMovieAsSeen(movieId, cancellationToken);

			TempData[TempDataMarkedMovieAsSeen] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(ApplicationPermissions.MoviesToSee.Delete)]
		public async Task<ViewResult> ConfirmMovieDeletion([FromRoute] string id, CancellationToken cancellationToken)
		{
			var viewModel = await CreateMovieViewModel(id, cancellationToken);
			return View(viewModel);
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToSee.Delete)]
		public async Task<RedirectToActionResult> DeleteMovie([FromForm] string id, CancellationToken cancellationToken)
		{
			var movieId = CreateMovieId(id);
			await moviesToSeeService.DeleteMovie(movieId, cancellationToken);

			TempData[TempDataDeletedMovie] = true;

			return RedirectToAction("Index");
		}

		protected override IQueryable<MovieToSeeModel> GetAllMovies()
		{
			return moviesToSeeService.GetAllMovies();
		}

		protected override MoviesToSeeViewModel CreateMoviesPageViewModel(IEnumerable<MovieToSeeModel> movies, int pageNumber, int totalPagesNumber)
		{
			return new(movies, pageNumber, totalPagesNumber)
			{
				ErrorMessage = TempData.GetStringValue(TempDataErrorMessage),
				AddedMovie = TempData.GetBooleanValue(TempDataAddedMovie),
				MarkedMovieAsSeen = TempData.GetBooleanValue(TempDataMarkedMovieAsSeen),
				DeletedMovie = TempData.GetBooleanValue(TempDataDeletedMovie),
			};
		}

		private async Task<MovieToSeeViewModel> CreateMovieViewModel(string id, CancellationToken cancellationToken)
		{
			var movieId = CreateMovieId(id);
			var movie = await moviesToSeeService.GetMovie(movieId, cancellationToken);
			return new MovieToSeeViewModel(movie);
		}
	}
}
