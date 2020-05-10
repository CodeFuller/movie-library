using MongoDB.Bson;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.Dal.MongoDB.Internal
{
	internal class AutoIdGenerator : IIdGenerator<ObjectId>
	{
		public ObjectId GenerateId()
		{
			return ObjectId.Empty;
		}
	}
}
