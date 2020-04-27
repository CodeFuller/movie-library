using System;
using MongoDB.Bson;

namespace MovieLibrary.Dal.MongoDB.Documents
{
	internal class MovieToSeeDocument
	{
		public ObjectId Id { get; set; }

		public DateTimeOffset TimestampOfAddingToSeeList { get; set; }

		public MovieInfoDocument MovieInfo { get; set; }
	}
}
