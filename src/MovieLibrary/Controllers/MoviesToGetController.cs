using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using MovieLibrary.Models;

namespace MovieLibrary.Controllers
{
	public class MoviesToGetController : Controller
	{
		private readonly IMoviesToGetService service;

		public MoviesToGetController(IMoviesToGetService service)
		{
			this.service = service ?? throw new ArgumentNullException(nameof(service));
		}

		[HttpGet]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			var viewModel = await ReadMoviesToGet(cancellationToken);

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Index(MoviesToGetViewModel model, CancellationToken cancellationToken)
		{
			if (ModelState.IsValid)
			{
				var newMovieToGet = model.NewMovieToGet;
				await service.AddMovieByUrl(newMovieToGet.MovieUri, cancellationToken);

				ModelState.Clear();
			}

			var viewModel = await ReadMoviesToGet(cancellationToken);

			return View(viewModel);
		}

		[HttpGet]
		public async Task<IActionResult> MoveToMoviesToSee(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.MoveToMoviesToSee(movieId, cancellationToken);

			var viewModel = await ReadMoviesToGet(cancellationToken);

			return View("Index", viewModel);
		}

		private async Task<MoviesToGetViewModel> ReadMoviesToGet(CancellationToken cancellationToken)
		{
			var moviesToGet = await service.GetAllMovies(cancellationToken).ToListAsync(cancellationToken);

			return new MoviesToGetViewModel(moviesToGet);
		}
	}
}
