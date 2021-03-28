using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieLibrary.Controllers;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.UnitTests.Controllers
{
	[TestClass]
	public class BasicMovieControllerTests
	{
		private class MoviesPageViewModel
		{
			public IReadOnlyCollection<string> Movies { get; }

			public int PageNumber { get; }

			public int TotalPagesNumber { get; }

			public MoviesPageViewModel(IEnumerable<string> movies, int pageNumber, int totalPagesNumber)
			{
				Movies = movies?.ToList() ?? throw new ArgumentNullException(nameof(movies));
				PageNumber = pageNumber;
				TotalPagesNumber = totalPagesNumber;
			}
		}

		private class ConcreteMoviesController : BasicMovieController<string, MoviesPageViewModel>
		{
			private readonly List<string> movies;

			protected override string ControllerName => "ConcreteMovies";

			public ConcreteMoviesController(IEnumerable<string> movies, int pageSize)
				: base(Mock.Of<IMovieUniquenessChecker>(), CreateOptions(pageSize))
			{
				this.movies = movies?.ToList() ?? throw new ArgumentNullException(nameof(movies));
			}

			protected override IQueryable<string> GetAllMovies()
			{
				return movies.AsQueryable();
			}

			protected override MoviesPageViewModel CreateMoviesPageViewModel(IEnumerable<string> movies, int pageNumber, int totalPagesNumber)
			{
				return new MoviesPageViewModel(movies, pageNumber, totalPagesNumber);
			}

			public IActionResult InvokeMoviesPageView(int pageNumber)
			{
				return MoviesPageView(pageNumber);
			}

			private static IOptions<AppSettings> CreateOptions(int pageSize)
			{
				var settings = new AppSettings
				{
					MoviesPageSize = pageSize,
				};

				return Options.Create(settings);
			}
		}

		[TestMethod]
		public void MoviesPageView_FirstPageRequestedWhenNoMoviesExist_ReturnsViewWithNoMovies()
		{
			// Arrange

			using var target = new ConcreteMoviesController(Enumerable.Empty<string>(), 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(1);

			// Assert

			var viewModel = ExtractViewModel(actionResult);

			CollectionAssert.AreEqual(Array.Empty<string>(), viewModel.Movies.ToList());
			Assert.AreEqual(1, viewModel.PageNumber);
			Assert.AreEqual(0, viewModel.TotalPagesNumber);
		}

		[TestMethod]
		public void MoviesPageView_SecondPageRequestedWhenNoMoviesExist_RedirectsToIndex()
		{
			// Arrange

			using var target = new ConcreteMoviesController(Enumerable.Empty<string>(), 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(2);

			// Assert

			var redirectPageNumber = ExtractRedirectPageNumber(actionResult);
			Assert.IsNull(redirectPageNumber);
		}

		[TestMethod]
		public void MoviesPageView_SecondPageRequestedWhenTotalOnePage_RedirectsToFirstPage()
		{
			// Arrange

			using var target = new ConcreteMoviesController(new[] { "Movie 1" }, 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(2);

			// Assert

			var redirectPageNumber = ExtractRedirectPageNumber(actionResult);
			Assert.AreEqual(1, redirectPageNumber);
		}

		[TestMethod]
		public void MoviesPageView_ThirdPageRequestedWhenTotalTwoPages_RedirectsToSecondPage()
		{
			// Arrange

			using var target = new ConcreteMoviesController(new[] { "Movie 1", "Movie 2", "Movie 3" }, 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(3);

			// Assert

			var redirectPageNumber = ExtractRedirectPageNumber(actionResult);
			Assert.AreEqual(2, redirectPageNumber);
		}

		[TestMethod]
		public void MoviesPageView_ZeroPageRequestedWhenTotalOnePage_RedirectsToIndex()
		{
			// Arrange

			using var target = new ConcreteMoviesController(new[] { "Movie 1" }, 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(0);

			// Assert

			var redirectPageNumber = ExtractRedirectPageNumber(actionResult);
			Assert.IsNull(redirectPageNumber);
		}

		[TestMethod]
		public void MoviesPageView_FirstPageRequestedWhenTotalThreePages_ReturnsCorrectMovies()
		{
			// Arrange

			using var target = new ConcreteMoviesController(new[] { "Movie 1", "Movie 2", "Movie 3", "Movie 4", "Movie 5" }, 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(1);

			// Assert

			var viewModel = ExtractViewModel(actionResult);

			CollectionAssert.AreEqual(new[] { "Movie 1", "Movie 2" }, viewModel.Movies.ToList());
			Assert.AreEqual(1, viewModel.PageNumber);
			Assert.AreEqual(3, viewModel.TotalPagesNumber);
		}

		[TestMethod]
		public void MoviesPageView_SecondPageRequestedWhenTotalThreePages_ReturnsCorrectMovies()
		{
			// Arrange

			using var target = new ConcreteMoviesController(new[] { "Movie 1", "Movie 2", "Movie 3", "Movie 4", "Movie 5" }, 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(2);

			// Assert

			var viewModel = ExtractViewModel(actionResult);

			CollectionAssert.AreEqual(new[] { "Movie 3", "Movie 4" }, viewModel.Movies.ToList());
			Assert.AreEqual(2, viewModel.PageNumber);
			Assert.AreEqual(3, viewModel.TotalPagesNumber);
		}

		[TestMethod]
		public void MoviesPageView_ThirdPageRequestedWhenTotalThreePages_ReturnsCorrectMovies()
		{
			// Arrange

			using var target = new ConcreteMoviesController(new[] { "Movie 1", "Movie 2", "Movie 3", "Movie 4", "Movie 5" }, 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(3);

			// Assert

			var viewModel = ExtractViewModel(actionResult);

			CollectionAssert.AreEqual(new[] { "Movie 5" }, viewModel.Movies.ToList());
			Assert.AreEqual(3, viewModel.PageNumber);
			Assert.AreEqual(3, viewModel.TotalPagesNumber);
		}

		private static MoviesPageViewModel ExtractViewModel(IActionResult actionResult)
		{
			Assert.IsInstanceOfType(actionResult, typeof(ViewResult));
			var viewResult = (ViewResult)actionResult;

			Assert.IsInstanceOfType(viewResult.Model, typeof(MoviesPageViewModel));
			return (MoviesPageViewModel)viewResult.Model;
		}

		private static int? ExtractRedirectPageNumber(IActionResult actionResult)
		{
			Assert.IsInstanceOfType(actionResult, typeof(RedirectToActionResult));
			var redirectResult = (RedirectToActionResult)actionResult;

			Assert.AreEqual("ConcreteMovies", redirectResult.ControllerName);
			Assert.AreEqual("Index", redirectResult.ActionName);

			if (redirectResult.RouteValues == null)
			{
				return null;
			}

			Assert.AreEqual(1, redirectResult.RouteValues.Count);

			var routeValue = redirectResult.RouteValues.Single();
			Assert.AreEqual("pageNumber", routeValue.Key);

			return (int)routeValue.Value;
		}
	}
}
