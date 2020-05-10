using System.Collections.Generic;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal interface IFakeIdGenerator<TId> : IIdGenerator<TId>
	{
		void SeedIds(IEnumerable<TId> ids);
	}
}
