using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MovieLibrary.UserManagement.Interfaces;

namespace MovieLibrary.UserManagement
{
	internal class RoleManagerWrapper<TRole, TKey> : RoleManager<TRole>, IRoleManager<TRole, TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		public RoleManagerWrapper(IRoleStore<TRole> store, IEnumerable<IRoleValidator<TRole>> roleValidators,
			ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<TRole>> logger)
			: base(store, roleValidators, keyNormalizer, errors, logger)
		{
		}
	}
}
