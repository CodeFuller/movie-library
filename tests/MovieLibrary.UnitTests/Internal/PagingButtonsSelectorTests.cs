using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Internal;

namespace MovieLibrary.UnitTests.Internal
{
	[TestClass]
	public class PagingButtonsSelectorTests
	{
		[DataRow(1, 1, 1)]
		[DataRow(1, 2, 1, 2)]
		[DataRow(2, 2, 1, 2)]
		[DataRow(1, 3, 1, 2, 3)]
		[DataRow(2, 3, 1, 2, 3)]
		[DataRow(3, 3, 1, 2, 3)]
		[DataRow(1, 4, 1, 2, 3, 4)]
		[DataRow(2, 4, 1, 2, 3, 4)]
		[DataRow(3, 4, 1, 2, 3, 4)]
		[DataRow(4, 4, 1, 2, 3, 4)]
		[DataRow(1, 5, 1, 2, 0, 5)]
		[DataRow(2, 5, 1, 2, 3, 4, 5)]
		[DataRow(3, 5, 1, 2, 3, 4, 5)]
		[DataRow(4, 5, 1, 2, 3, 4, 5)]
		[DataRow(5, 5, 1, 0, 4, 5)]
		[DataRow(1, 6, 1, 2, 0, 6)]
		[DataRow(2, 6, 1, 2, 3, 0, 6)]
		[DataRow(3, 6, 1, 2, 3, 4, 5, 6)]
		[DataRow(4, 6, 1, 2, 3, 4, 5, 6)]
		[DataRow(5, 6, 1, 0, 4, 5, 6)]
		[DataRow(6, 6, 1, 0, 5, 6)]
		[DataTestMethod]
		public void SelectButtonsPages_SelectsCorrectButtons(int currentPageNumber, int totalPagesNumber, params int[] expectedSelectedButtons)
		{
			// Arrange

			// Act

			var selectedPages = PagingButtonsSelector.SelectButtonsPages(currentPageNumber, totalPagesNumber);

			// Assert

			selectedPages.Should().Equal(expectedSelectedButtons);
		}
	}
}
