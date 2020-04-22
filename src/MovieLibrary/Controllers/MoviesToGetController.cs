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
		private readonly IMoviesToGetRepository repository;

		public MoviesToGetController(IMoviesToGetRepository repository)
		{
			this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		[HttpGet]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			var moviesToGet = await repository.ReadMoviesToGet(cancellationToken).ToListAsync(cancellationToken);

			var model = new MoviesToGetModel
			{
				Movies = moviesToGet.Select(m => new MovieToGetModel
				{
					Title = m.Title,
				}).ToList(),
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Index(MoviesToGetModel model, CancellationToken cancellationToken)
		{
			if (ModelState.IsValid)
			{
				await repository.CreateMovieToGet(model.NewMovieToGet, cancellationToken);

				ModelState.Clear();
			}

			var moviesToGet = await repository.ReadMoviesToGet(cancellationToken).ToListAsync(cancellationToken);

			var outputModel = new MoviesToGetModel
			{
				Movies = moviesToGet,
			};

			return View(outputModel);
		}
	}
}
