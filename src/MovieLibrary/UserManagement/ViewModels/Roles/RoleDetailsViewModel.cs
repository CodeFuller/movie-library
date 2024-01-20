using System.Collections.Generic;
using System.Linq;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.ViewModels.Roles
{
	public class RoleDetailsViewModel : IPermissionsHolder
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
