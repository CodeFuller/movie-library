using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class FakeIdGenerator<TId> : IFakeIdGenerator<TId>
	{
		private readonly Queue<TId> idsQueue = new Queue<TId>();

		public void SeedIds(IEnumerable<TId> ids)
		{
			if (idsQueue.Any())
			{
				throw new InvalidOperationException("Not all seeded ids were used");
			}

			foreach (var id in ids)
			{
				idsQueue.Enqueue(id);
			}
		}

		public TId GenerateId()
		{
			if (!idsQueue.Any())
			{
				throw new InvalidOperationException("The are no more seeded ids");
			}

			return idsQueue.Dequeue();
		}
	}
}
