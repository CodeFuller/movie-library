using System.Runtime.Serialization;

namespace MovieLibrary.Logic.Kinopoisk.DataContracts
{
	[DataContract]
	internal class FilmRatingDataContract
	{
		[DataMember(Name = "rating")]
		public decimal? Rating { get; set; }

		// For films with vote count < 1000, RatingVoteCount is return in number format.
		// For films with vote count > 1000, RatingVoteCount is return in string format ("12 345").
		[DataMember(Name = "ratingVoteCount")]
		public string RatingVoteCount { get; set; }
	}
}
