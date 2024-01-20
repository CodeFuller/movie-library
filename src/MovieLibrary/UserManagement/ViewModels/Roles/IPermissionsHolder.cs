using System.Collections.Generic;

namespace MovieLibrary.UserManagement.ViewModels.Roles
{
	public interface IPermissionsHolder
	{
		public IReadOnlyList<RolePermissionViewModel> Permissions { get; set; }
	}
}
