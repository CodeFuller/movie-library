using System.Collections.Generic;

namespace MovieLibrary.Internal
{
	public static class PagingButtonsSelector
	{
		public static IReadOnlyCollection<int> SelectButtonsPages(int currentPageNumber, int totalPagesNumber)
		{
			var selectedPages = new List<int>(7);

			// First page
			AddPage(selectedPages, 1, totalPagesNumber);

			// Dots before first page
			if (currentPageNumber - 2 > 1)
			{
				// If there is actually one page behind the dots, we show it's number, not the dots
				if (currentPageNumber - 2 == 2)
				{
					AddPage(selectedPages, 2, totalPagesNumber);
				}
				else
				{
					selectedPages.Add(0);
				}
			}

			// Previous page
			AddPage(selectedPages, currentPageNumber - 1, totalPagesNumber);

			// Current page
			AddPage(selectedPages, currentPageNumber, totalPagesNumber);

			// Next page
			AddPage(selectedPages, currentPageNumber + 1, totalPagesNumber);

			// Dots before last page
			if (currentPageNumber + 2 < totalPagesNumber)
			{
				// If there is actually one page behind the dots, we show it's number, not the dots
				if (currentPageNumber + 2 == totalPagesNumber - 1)
				{
					AddPage(selectedPages, totalPagesNumber - 1, totalPagesNumber);
				}
				else
				{
					selectedPages.Add(0);
				}
			}

			// Last page
			AddPage(selectedPages, totalPagesNumber, totalPagesNumber);

			return selectedPages;
		}

		private static void AddPage(List<int> selectedPages, int page, int totalPagesNumber)
		{
			if (page < 1 || page > totalPagesNumber)
			{
				return;
			}

			if (selectedPages.Contains(page))
			{
				return;
			}

			selectedPages.Add(page);
		}
	}
}
