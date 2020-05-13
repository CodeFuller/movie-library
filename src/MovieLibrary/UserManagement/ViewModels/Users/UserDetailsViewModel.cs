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

		public UserDetailsViewModel(UserModel model, IEnumerable<string> userRoles, IEnumerable<string> allRoles)
		{
			UserId = model.Id;
			UserName = model.UserName;

			Roles = allRoles
				.Select(role => new UserRoleViewModel
				{
					RoleName = role,
					Assigned = userRoles.Contains(role),
				})
				.ToList();
		}
	}
}
