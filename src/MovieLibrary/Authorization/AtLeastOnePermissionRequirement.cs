using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace MovieLibrary.Authorization
{
	internal class AtLeastOnePermissionRequirement : IAuthorizationRequirement
	{
		public IReadOnlyCollection<string> Permissions { get; }

		public AtLeastOnePermissionRequirement(IEnumerable<string> permissions)
		{
			Permissions = permissions?.ToList() ?? throw new ArgumentNullException(nameof(permissions));
		}
	}
}
