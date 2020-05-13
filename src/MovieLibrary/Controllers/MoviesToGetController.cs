﻿using System;
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
		private const string TempDataAddedMovie = "AddedMovie";
		private const string TempDataMovedMovie = "MovedMovie";
		private const string TempDataDeletedMovie = "DeletedMovie";

		private readonly IMoviesToGetService moviesToGetService;

		private readonly IMovieInfoService movieInfoService;

		protected override string ControllerName => "MoviesToGet";

		public MoviesToGetController(IMoviesToGetService moviesToGetService, IMovieInfoService movieInfoService, IOptions<AppSettings> options)
			: base(options)
		{
			this.moviesToGetService = moviesToGetService ?? throw new ArgumentNullException(nameof(moviesToGetService));
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
			var movieInfo = await movieInfoService.LoadMovieInfoByUrl(newMovieToGet.MovieUri, cancellationToken);

			return View("ConfirmMovieAdding", new InputMovieInfoViewModel(movieInfo));
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToGet.Add)]
		public async Task<IActionResult> AddMovie([FromForm] InputMovieInfoViewModel model, CancellationToken cancellationToken)
		{
			var movieInfo = model.ToMovieInfo();
			await moviesToGetService.AddMovie(movieInfo, cancellationToken);

			TempData[TempDataAddedMovie] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(ApplicationPermissions.MoviesToGet.MoveToMoviesToSee)]
		public async Task<IActionResult> ConfirmMovingToSee([FromRoute] string id, CancellationToken cancellationToken)
		{
			var viewModel = await CreateMovieViewModel(id, cancellationToken);
			return View(viewModel);
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToGet.MoveToMoviesToSee)]
		public async Task<IActionResult> MoveToMoviesToSee([FromForm] string id, CancellationToken cancellationToken)
		{
			var movieId = CreateMovieId(id);
			await moviesToGetService.MoveToMoviesToSee(movieId, cancellationToken);

			TempData[TempDataMovedMovie] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(ApplicationPermissions.MoviesToGet.Delete)]
		public async Task<IActionResult> ConfirmMovieDeletion([FromRoute] string id, CancellationToken cancellationToken)
		{
			var viewModel = await CreateMovieViewModel(id, cancellationToken);
			return View(viewModel);
		}

		[HttpPost]
		[Authorize(ApplicationPermissions.MoviesToGet.Delete)]
		public async Task<IActionResult> DeleteMovie([FromForm] string id, CancellationToken cancellationToken)
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
			return new MoviesToGetViewModel(movies, pageNumber, totalPagesNumber)
			{
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
