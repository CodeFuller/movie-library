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
			var moviesToGet = await service.GetMoviesToGet(cancellationToken).ToListAsync(cancellationToken);

			var model = new MoviesToGetModel
			{
				Movies = moviesToGet.Select(m => new MovieToGetModel(m.Id, m.MovieInfo)).ToList(),
			};

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

			var moviesToGet = await service.GetMoviesToGet(cancellationToken).ToListAsync(cancellationToken);

			var outputModel = new MoviesToGetModel
			{
				Movies = moviesToGet.Select(m => new MovieToGetModel(m.Id, m.MovieInfo)).ToList(),
			};

			return View(outputModel);
		}
	}
}
