using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.UserManagement.ViewModels.Roles
{
	public class NewRoleViewModel
	{
		[Required]
		[Display(Name = "Name")]
		public string Name { get; set; }

		public IReadOnlyList<RolePermissionViewModel> Permissions { get; set; }
	}
}
