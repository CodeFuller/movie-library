using MongoDB.Bson;

namespace MovieLibrary.Dal.MongoDB.Internal
{
	internal class AutoIdGenerator : IDocumentIdGenerator
	{
		public ObjectId GenerateIdForNewDocument()
		{
			return ObjectId.Empty;
		}
	}
}
