using System;
using System.Collections.Generic;
using System.Linq;
using MovieLibrary.UserManagement.Models;
using MovieLibrary.UserManagement.ViewModels.Users;

namespace MovieLibrary.UserManagement.ViewModels.Roles
{
	public class RolesListViewModel
	{
		public IReadOnlyCollection<RoleViewModel> Roles { get; }

		public bool AddedRole { get; set; }

		public bool UpdatedRole { get; set; }

		public bool DeletedRole { get; set; }

		public RolesListViewModel(IEnumerable<RoleModel> roles)
		{
			Roles = roles?.Select(r => new RoleViewModel(r)).ToList() ?? throw new ArgumentNullException(nameof(roles));
		}
	}
}
