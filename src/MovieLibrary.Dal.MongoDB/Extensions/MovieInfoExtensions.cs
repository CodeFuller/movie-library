using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Dal.MongoDB.Extensions
{
	internal static class MovieInfoExtensions
	{
		public static MovieInfoDocument ToDocument(this MovieInfoModel model)
		{
			return new MovieInfoDocument
			{
				Title = model.Title,
				Year = model.Year,
				MovieUri = model.MovieUri,
				PosterUri = model.PosterUri,
				Directors = model.Directors,
				Cast = model.Cast,
				RatingValue = model.Rating?.Value,
				RatingVotesNumber = model.Rating?.VotesNumber,
				Duration = model.Duration,
				Genres = model.Genres,
				Summary = model.Summary,
			};
		}

		public static MovieInfoModel ToModel(this MovieInfoDocument document)
		{
			return new MovieInfoModel
			{
				Title = document.Title,
				Year = document.Year,
				MovieUri = document.MovieUri,
				PosterUri = document.PosterUri,
				Directors = document.Directors,
				Cast = document.Cast,
				Rating = document.RatingValue != null ? new MovieRatingModel(document.RatingValue.Value, document.RatingVotesNumber) : null,
				Duration = document.Duration,
				Genres = document.Genres,
				Summary = document.Summary,
			};
		}
	}
}
