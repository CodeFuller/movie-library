using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Extensions;
using MovieLibrary.Models;

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

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public ViewResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
