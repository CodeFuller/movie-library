using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Controllers;

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
				: base(CreateOptions(pageSize))
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

			var expectedViewModel = new MoviesPageViewModel(Array.Empty<string>(), 1, 0);

			var viewModel = ExtractViewModel(actionResult);
			viewModel.Should().BeEquivalentTo(expectedViewModel, x => x.WithStrictOrdering());
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
			redirectPageNumber.Should().BeNull();
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
			redirectPageNumber.Should().Be(1);
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
			redirectPageNumber.Should().Be(2);
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
			redirectPageNumber.Should().BeNull();
		}

		[TestMethod]
		public void MoviesPageView_FirstPageRequestedWhenTotalThreePages_ReturnsCorrectMovies()
		{
			// Arrange

			using var target = new ConcreteMoviesController(new[] { "Movie 1", "Movie 2", "Movie 3", "Movie 4", "Movie 5" }, 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(1);

			// Assert

			var expectedViewModel = new MoviesPageViewModel(new[] { "Movie 1", "Movie 2" }, 1, 3);

			var viewModel = ExtractViewModel(actionResult);
			viewModel.Should().BeEquivalentTo(expectedViewModel, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void MoviesPageView_SecondPageRequestedWhenTotalThreePages_ReturnsCorrectMovies()
		{
			// Arrange

			using var target = new ConcreteMoviesController(new[] { "Movie 1", "Movie 2", "Movie 3", "Movie 4", "Movie 5" }, 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(2);

			// Assert

			var expectedViewModel = new MoviesPageViewModel(new[] { "Movie 3", "Movie 4" }, 2, 3);

			var viewModel = ExtractViewModel(actionResult);
			viewModel.Should().BeEquivalentTo(expectedViewModel, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void MoviesPageView_ThirdPageRequestedWhenTotalThreePages_ReturnsCorrectMovies()
		{
			// Arrange

			using var target = new ConcreteMoviesController(new[] { "Movie 1", "Movie 2", "Movie 3", "Movie 4", "Movie 5" }, 2);

			// Act

			var actionResult = target.InvokeMoviesPageView(3);

			// Assert

			var expectedViewModel = new MoviesPageViewModel(new[] { "Movie 5" }, 3, 3);

			var viewModel = ExtractViewModel(actionResult);
			viewModel.Should().BeEquivalentTo(expectedViewModel, x => x.WithStrictOrdering());
		}

		private static MoviesPageViewModel ExtractViewModel(IActionResult actionResult)
		{
			actionResult.Should().BeOfType<ViewResult>();
			var viewResult = (ViewResult)actionResult;

			viewResult.Model.Should().BeOfType<MoviesPageViewModel>();
			return (MoviesPageViewModel)viewResult.Model;
		}

		private static int? ExtractRedirectPageNumber(IActionResult actionResult)
		{
			actionResult.Should().BeOfType<RedirectToActionResult>();
			var redirectResult = (RedirectToActionResult)actionResult;

			redirectResult.ControllerName.Should().Be("ConcreteMovies");
			redirectResult.ActionName.Should().Be("Index");

			if (redirectResult.RouteValues == null)
			{
				return null;
			}

			redirectResult.RouteValues.Keys.Should().Equal("pageNumber");
			return (int)redirectResult.RouteValues.Values.Single();
		}
	}
}
