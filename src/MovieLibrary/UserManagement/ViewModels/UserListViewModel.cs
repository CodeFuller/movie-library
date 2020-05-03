using System;
using System.Collections.Generic;
using System.Linq;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.ViewModels
{
	public class UserListViewModel
	{
		public IReadOnlyCollection<UserViewModel> Users { get; }

		public UserListViewModel(IEnumerable<UserModel> users)
		{
			Users = users?.Select(u => new UserViewModel(u)).ToList() ?? throw new ArgumentNullException(nameof(users));
		}
	}
}
