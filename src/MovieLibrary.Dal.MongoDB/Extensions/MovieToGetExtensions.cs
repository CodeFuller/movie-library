using MongoDB.Bson;
using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Dal.MongoDB.Extensions
{
	internal static class MovieToGetExtensions
	{
		public static MovieToGetDocument ToDocument(this MovieToGetModel model)
		{
			return new MovieToGetDocument
			{
				Id = model.Id != null ? new ObjectId(model.Id.Value) : default,
				TimestampOfAddingToGetList = model.TimestampOfAddingToGetList,
				MovieInfo = model.MovieInfo.ToDocument(),
			};
		}

		public static MovieToGetModel ToModel(this MovieToGetDocument document)
		{
			var id = document.Id.ToMovieId();
			var movieInfo = document.MovieInfo.ToModel();

			return new MovieToGetModel
			{
				Id = id,
				TimestampOfAddingToGetList = document.TimestampOfAddingToGetList,
				MovieInfo = movieInfo,
			};
		}
	}
}
