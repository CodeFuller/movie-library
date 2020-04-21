using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.Controllers
{
	public class MoviesToGetController : Controller
	{
		private readonly IMoviesToGetRepository repository;

		public MoviesToGetController(IMoviesToGetRepository repository)
		{
			this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			var moviesToGet = await repository.ReadMoviesToGet(cancellationToken).ToListAsync(cancellationToken);

			return View(moviesToGet);
		}
	}
}
