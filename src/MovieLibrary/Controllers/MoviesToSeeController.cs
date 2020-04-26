using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Models;

namespace MovieLibrary.Controllers
{
	public class MoviesToSeeController : Controller
	{
		private readonly IMoviesToSeeService service;

		public MoviesToSeeController(IMoviesToSeeService service)
		{
			this.service = service ?? throw new ArgumentNullException(nameof(service));
		}

		[HttpGet]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			var movies = await service.GetAllMovies(cancellationToken).ToListAsync(cancellationToken);

			var model = new MoviesToSeeViewModel(movies);

			return View(model);
		}
	}
}
