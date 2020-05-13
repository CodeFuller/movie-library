using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieLibrary.UserManagement.Models;
using MovieLibrary.UserManagement.ViewModels.Users;

namespace MovieLibrary.UserManagement.ViewModels.Roles
{
	public class RoleDetailsViewModel
	{
		public string RoleId { get; set; }

		public string RoleName { get; set; }

		public IReadOnlyList<RolePermissionViewModel> Permissions { get; set; }

		public RoleDetailsViewModel()
		{
		}

		public RoleDetailsViewModel(RoleModel model, IEnumerable<string> rolePermissions, IEnumerable<string> allPermissions)
		{
			RoleId = model.Id;
			RoleName = model.RoleName;

			Permissions = allPermissions
				.Select(permission => new RolePermissionViewModel
				{
					PermissionName = permission,
					Assigned = rolePermissions.Contains(permission),
				})
				.ToList();
		}
	}
}
