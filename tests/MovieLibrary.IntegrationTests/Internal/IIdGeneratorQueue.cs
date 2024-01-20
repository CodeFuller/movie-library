using System.Collections.Generic;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal interface IIdGeneratorQueue
	{
		void EnqueueId(string id);

		void EnqueueIds(IEnumerable<string> ids);
	}
}
