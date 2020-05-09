using MongoDB.Bson;
using MovieLibrary.Dal.MongoDB.Internal;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class FakeIdGenerator : IDocumentIdGenerator
	{
		public ObjectId GenerateIdForNewDocument()
		{
			return new ObjectId("5eb706d725d7b94ebc88af81");
		}
	}
}
