using System.Collections.Generic;
using System.Linq;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.ViewModels
{
	public class UserDetailsViewModel
	{
		public string UserId { get; set; }

		public string UserName { get; set; }

		public IReadOnlyList<UserPermissionViewModel> Permissions { get; set; }

		public UserDetailsViewModel()
		{
		}

		public UserDetailsViewModel(UserDetailsModel model)
		{
			UserId = model.Id;
			UserName = model.UserName;
			Permissions = model.AllPermissions
				.Select(p => new UserPermissionViewModel
				{
					PermissionName = p,
					Assigned = model.UserPermissions.Contains(p),
				})
				.ToList();
		}
	}
}
