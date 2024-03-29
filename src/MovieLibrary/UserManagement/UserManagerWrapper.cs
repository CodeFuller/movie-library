using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieLibrary.UserManagement.Interfaces;

namespace MovieLibrary.UserManagement
{
	internal class UserManagerWrapper<TUser, TKey> : UserManager<TUser>, IUserManager<TUser, TKey>
		where TUser : IdentityUser<TKey>
		where TKey : IEquatable<TKey>
	{
		public UserManagerWrapper(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher,
			IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators,
			ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger)
			: base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		{
		}
	}
}
