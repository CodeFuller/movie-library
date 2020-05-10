using System;
using MongoDB.Bson;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class FakeIdGenerator : IIdGeneratorQueue, IIdGenerator<ObjectId>
	{
		private string NextId { get; set; }

		public void SetNextId(string id)
		{
			if (NextId != null)
			{
				throw new InvalidOperationException("Previous fake id was not used");
			}

			NextId = id;
		}

		public ObjectId GenerateId()
		{
			if (NextId == null)
			{
				throw new InvalidOperationException("Fake id was not set");
			}

			var id = new ObjectId(NextId);

			NextId = null;

			return id;
		}
	}
}
