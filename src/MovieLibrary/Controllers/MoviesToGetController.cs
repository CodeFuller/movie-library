using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

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
			var model = await GetMoviesToGet(cancellationToken);

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Index(MoviesToGetModel model, CancellationToken cancellationToken)
		{
			if (ModelState.IsValid)
			{
				var newMovieToGet = model.NewMovieToGet;
				await service.AddMovieToGetByUrl(newMovieToGet.MovieUri, cancellationToken);

				ModelState.Clear();
			}

			var outputModel = await GetMoviesToGet(cancellationToken);

			return View(outputModel);
		}

		[HttpGet]
		public async Task<IActionResult> MoveToMoviesToSee(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));
			var movieId = new MovieId(id);

			await service.MoveToMoviesToSee(movieId, cancellationToken);

			var model = await GetMoviesToGet(cancellationToken);

			return View("Index", model);
		}

		private async Task<MoviesToGetModel> GetMoviesToGet(CancellationToken cancellationToken)
		{
			var moviesToGet = await service.GetMoviesToGet(cancellationToken).ToListAsync(cancellationToken);

			return new MoviesToGetModel
			{
				Movies = moviesToGet,
			};
		}
	}
}
