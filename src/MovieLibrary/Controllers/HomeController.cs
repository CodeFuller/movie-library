using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Extensions;

namespace MovieLibrary.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			if (User.CanAccessMoviesToSee())
			{
				return RedirectToAction("Index", "MoviesToSee");
			}

			if (User.CanAccessMoviesToGet())
			{
				return RedirectToAction("Index", "MoviesToGet");
			}

			if (User.CanManagerUsers())
			{
				return RedirectToAction("Index", "Users");
			}

			return Forbid();
		}
	}
}
