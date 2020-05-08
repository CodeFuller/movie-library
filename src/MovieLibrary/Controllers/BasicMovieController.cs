using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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

			if (pageNumber < 1)
			{
				return RedirectToAction("Index", ControllerName, new { pageNumber = 1 });
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
	}
}
