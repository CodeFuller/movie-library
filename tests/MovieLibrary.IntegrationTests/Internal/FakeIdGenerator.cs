using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class FakeIdGenerator : IIdGeneratorQueue, IIdGenerator<ObjectId>
	{
		private readonly Queue<string> idsQueue = new Queue<string>();

		public void EnqueueId(string id)
		{
			EnqueueIds(new[] { id });
		}

		public void EnqueueIds(IEnumerable<string> ids)
		{
			if (idsQueue.Any())
			{
				throw new InvalidOperationException("Previous fake ids were not used");
			}

			foreach (var id in ids)
			{
				idsQueue.Enqueue(id);
			}
		}

		public ObjectId GenerateId()
		{
			if (!idsQueue.Any())
			{
				throw new InvalidOperationException("Fake id was not set");
			}

			return new ObjectId(idsQueue.Dequeue());
		}
	}
}
