using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

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
			var moviesToSee = await service.GetMoviesToSee(cancellationToken).ToListAsync(cancellationToken);

			var model = new MoviesToSeeModel
			{
				Movies = moviesToSee,
			};

			return View(model);
		}
	}
}
