using System;
using System.Collections.Generic;
using System.Linq;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.ViewModels.Users
{
	public class UsersListViewModel
	{
		public IReadOnlyCollection<UserViewModel> Users { get; }

		public bool AddedUser { get; set; }

		public bool UpdatedUser { get; set; }

		public bool DeletedUser { get; set; }

		public UsersListViewModel(IEnumerable<UserModel> users)
		{
			Users = users?.Select(u => new UserViewModel(u)).ToList() ?? throw new ArgumentNullException(nameof(users));
		}
	}
}
