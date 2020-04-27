using System;
using MongoDB.Bson;

namespace MovieLibrary.Dal.MongoDB.Documents
{
	internal class MovieToGetDocument
	{
		public ObjectId Id { get; set; }

		public DateTimeOffset TimestampOfAddingToGetList { get; set; }

		public MovieInfoDocument MovieInfo { get; set; }
	}
}
