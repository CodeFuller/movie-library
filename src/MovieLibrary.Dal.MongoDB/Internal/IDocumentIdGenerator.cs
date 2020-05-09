using MongoDB.Bson;

namespace MovieLibrary.Dal.MongoDB.Internal
{
	internal interface IDocumentIdGenerator
	{
		ObjectId GenerateIdForNewDocument();
	}
}
