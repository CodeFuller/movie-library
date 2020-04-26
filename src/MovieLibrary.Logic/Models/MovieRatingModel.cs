namespace MovieLibrary.Logic.Models
{
	public class MovieRatingModel
	{
		public decimal Value { get; set; }

		public int? VotesNumber { get; set; }

		public MovieRatingModel(decimal value, int? votesNumber)
		{
			Value = value;
			VotesNumber = votesNumber;
		}
	}
}
