using MongoDB.Bson;
using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Dal.MongoDB.Extensions
{
	internal static class MovieToSeeExtensions
	{
		public static MovieToSeeDocument ToDocument(this MovieToSeeModel model)
		{
			return new MovieToSeeDocument
			{
				Id = model.Id != null ? new ObjectId(model.Id.Value) : default,
				TimestampOfAddingToSeeList = model.TimestampOfAddingToSeeList,
				MovieInfo = model.MovieInfo.ToDocument(),
			};
		}

		public static MovieToSeeModel ToModel(this MovieToSeeDocument document)
		{
			var id = document.Id.ToMovieId();
			var movieInfo = document.MovieInfo.ToModel();

			return new MovieToSeeModel
			{
				Id = id,
				TimestampOfAddingToSeeList = document.TimestampOfAddingToSeeList,
				MovieInfo = movieInfo,
			};
		}
	}
}
