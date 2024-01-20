using System.Collections.Generic;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal sealed class UserSeedData
	{
		public string Id { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		public IReadOnlyCollection<string> Roles { get; set; }
	}
}
