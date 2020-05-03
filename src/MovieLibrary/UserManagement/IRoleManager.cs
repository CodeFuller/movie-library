using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace MovieLibrary.UserManagement
{
	internal interface IRoleManager<TRole, TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		IQueryable<TRole> Roles { get; }
	}
}
