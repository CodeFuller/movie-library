using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Models;

namespace MovieLibrary.Controllers
{
	[AllowAnonymous]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ErrorsController : Controller
	{
		public ViewResult ErrorCode([FromQuery] HttpStatusCode statusCode)
		{
			return ErrorView(statusCode);
		}

		public ViewResult UnhandledException()
		{
			return ErrorView(HttpStatusCode.InternalServerError);
		}

		private ViewResult ErrorView(HttpStatusCode statusCode)
		{
			var viewModel = new ErrorViewModel
			{
				StatusCode = statusCode,
			};

			var viewName = GetErrorViewName(statusCode);

			return View(viewName, viewModel);
		}

		private static string GetErrorViewName(HttpStatusCode statusCode)
		{
			return statusCode switch
			{
				HttpStatusCode.NotFound => "Error404",
				HttpStatusCode.InternalServerError => "Error500",
				_ => "Error",
			};
		}
	}
}
