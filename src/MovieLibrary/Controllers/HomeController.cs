﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Models;

namespace MovieLibrary.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return RedirectToAction("Index", "MoviesToSee");
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
