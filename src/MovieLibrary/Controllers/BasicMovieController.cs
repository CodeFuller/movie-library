using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Controllers
{
	public abstract class BasicMovieController<TMovieModel, TMoviesViewModel> : Controller
	{
		protected const string TempDataErrorMessage = "Error";

		private readonly IMovieUniquenessChecker movieUniquenessChecker;

		private readonly AppSettings settings;

		protected BasicMovieController(IMovieUniquenessChecker movieUniquenessChecker, IOptions<AppSettings> options)
		{
			this.movieUniquenessChecker = movieUniquenessChecker ?? throw new ArgumentNullException(nameof(movieUniquenessChecker));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		protected abstract string ControllerName { get; }

		protected abstract IQueryable<TMovieModel> GetAllMovies();

		protected abstract TMoviesViewModel CreateMoviesPageViewModel(IEnumerable<TMovieModel> movies, int pageNumber, int totalPagesNumber);

		protected IActionResult MoviesPageView(int pageNumber)
		{
			var moviesQueryable = GetAllMovies();

			var moviesCount = moviesQueryable.Count();
			var totalPagesNumber = (int)Math.Ceiling(moviesCount / (double)settings.MoviesPageSize);

			if (totalPagesNumber == 0 && pageNumber != 1)
			{
				return RedirectToAction("Index", ControllerName);
			}

			if (pageNumber < 1)
			{
				return RedirectToAction("Index", ControllerName);
			}

			if (pageNumber > totalPagesNumber && totalPagesNumber != 0)
			{
				return RedirectToAction("Index", ControllerName, new { pageNumber = totalPagesNumber });
			}

			return CreateMoviesPageView(moviesQueryable, pageNumber, totalPagesNumber);
		}

		private ViewResult CreateMoviesPageView(IQueryable<TMovieModel> movies, int pageNumber, int totalPagesNumber)
		{
			var pageMovies = movies
				.Skip((pageNumber - 1) * settings.MoviesPageSize)
				.Take(settings.MoviesPageSize);

			var viewModel = CreateMoviesPageViewModel(pageMovies, pageNumber, totalPagesNumber);

			return View("Index", viewModel);
		}

		protected static MovieId CreateMovieId(string id)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			return new MovieId(id);
		}

		protected async Task<IActionResult> CheckMovieUniqueness(Uri movieUri, int pageNumber, CancellationToken cancellationToken)
		{
			var movieCheckResult = await movieUniquenessChecker.CheckMovie(movieUri, cancellationToken);
			if (movieCheckResult != MovieUniquenessCheckResult.MovieIsUnique)
			{
				return DuplicatedMovieError(movieCheckResult, movieUri, pageNumber);
			}

			return null;
		}

		private IActionResult DuplicatedMovieError(MovieUniquenessCheckResult checkResult, Uri movieUri, int pageNumber)
		{
			var errorMessage = checkResult switch
			{
				MovieUniquenessCheckResult.ExistsInMoviesToGet => $"Movie {movieUri} already exists among movies to get",
				MovieUniquenessCheckResult.ExistsInMoviesToSee => $"Movie {movieUri} already exists among movies to see",
				_ => throw new InvalidOperationException($"Unexpected value of movie check result: {checkResult}")
			};

			TempData[TempDataErrorMessage] = errorMessage;

			// Clearing movie URL from the input.
			ModelState.Clear();

			return MoviesPageView(pageNumber);
		}
	}
}
