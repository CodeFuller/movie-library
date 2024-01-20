using System.Collections.Generic;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal sealed class RoleSeedData
	{
		public string Id { get; set; }

		public string RoleName { get; set; }

		public IEnumerable<string> Permissions { get; set; }
	}
}
