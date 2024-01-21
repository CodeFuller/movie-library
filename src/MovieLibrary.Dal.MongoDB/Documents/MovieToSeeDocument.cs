using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieLibrary.Dal.MongoDB.Documents
{
	internal class MovieToSeeDocument
	{
		public ObjectId Id { get; set; }

		// We use string representation of DateTimeOffset instead of array serialization used by default.
		// For default array serialization, sorting by DateTimeOffset field works incorrectly, which shows up in incorrect behavior of paging.
		// See https://stackoverflow.com/a/61672913/5740031
		[BsonRepresentation(BsonType.String)]
		public DateTimeOffset TimestampOfAddingToSeeList { get; set; }

		public MovieInfoDocument MovieInfo { get; set; }

		public string Reference { get; set; }
	}
}
