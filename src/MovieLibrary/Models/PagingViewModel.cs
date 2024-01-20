namespace MovieLibrary.Models
{
	public class PagingViewModel
	{
		public int CurrentPageNumber { get; set; }

		public int TotalPagesNumber { get; set; }

		public PagingViewModel()
		{
		}

		public PagingViewModel(int currentPageNumber, int totalPagesNumber)
		{
			CurrentPageNumber = currentPageNumber;
			TotalPagesNumber = totalPagesNumber;
		}
	}
}
