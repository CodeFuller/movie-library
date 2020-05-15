using System;
using System.Collections.Generic;
using System.Linq;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.ViewModels.Users
{
	public class UserDetailsViewModel
	{
		public string UserId { get; set; }

		public string UserName { get; set; }

		public IReadOnlyList<UserRoleViewModel> Roles { get; set; }

		public UserDetailsViewModel()
		{
		}

		public UserDetailsViewModel(UserModel model, IEnumerable<UserRoleModel> userRoles, IEnumerable<string> allRoles)
		{
			UserId = model.Id;
			UserName = model.UserName;

			var userRolesSet = userRoles.ToHashSet();

			Roles = allRoles
				.Select(role => CreateUserRoleViewModel(role, userRolesSet))
				.ToList();
		}

		private static UserRoleViewModel CreateUserRoleViewModel(string role, HashSet<UserRoleModel> userRoles)
		{
			var userRole = userRoles.FirstOrDefault(r => String.Equals(r.RoleName, role, StringComparison.Ordinal));

			return new UserRoleViewModel
			{
				RoleName = role,
				Assigned = userRole != null,
				ReadOnly = userRole?.ReadOnly ?? false,
			};
		}
	}
}
