using System.Collections.Generic;

namespace MovieLibrary.UserManagement.ViewModels
{
	public class UserDetailsViewModel
	{
		public string UserId { get; set; }

		public string UserName { get; set; }

		public IReadOnlyList<UserPermissionViewModel> Permissions { get; set; }
	}
}
