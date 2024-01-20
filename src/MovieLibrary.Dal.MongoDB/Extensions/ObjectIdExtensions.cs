using MongoDB.Bson;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Dal.MongoDB.Extensions
{
	public static class ObjectIdExtensions
	{
		public static MovieId ToMovieId(this ObjectId id)
		{
			return new MovieId(id.ToString());
		}
	}
}
