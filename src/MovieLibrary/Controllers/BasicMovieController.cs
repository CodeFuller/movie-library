using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Controllers
{
	public abstract class BasicMovieController<TMovieModel, TMoviesViewModel> : Controller
	{
		private readonly AppSettings settings;

		protected BasicMovieController(IOptions<AppSettings> options)
		{
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

		protected static string GetDuplicatedMovieError(MovieUniquenessCheckResult checkResult, Uri movieUri)
		{
			return checkResult switch
			{
				MovieUniquenessCheckResult.ExistsInMoviesToGet => $"Movie {movieUri} already exists among movies to get",
				MovieUniquenessCheckResult.ExistsInMoviesToSee => $"Movie {movieUri} already exists among movies to see",
				_ => throw new InvalidOperationException($"Unexpected value of movie check result: {checkResult}"),
			};
		}
	}
}
