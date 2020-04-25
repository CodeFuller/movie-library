namespace MovieLibrary.Logic.MoviesInfo
{
	public class MovieRating
	{
		public decimal Value { get; set; }

		public int? VotesNumber { get; set; }

		public MovieRating(decimal value, int? votesNumber)
		{
			Value = value;
			VotesNumber = votesNumber;
		}
	}
}
