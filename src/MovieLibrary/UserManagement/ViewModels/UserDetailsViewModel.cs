using System.Collections.Generic;
using System.Linq;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.ViewModels
{
	public class UserDetailsViewModel
	{
		public string UserId { get; set; }

		public string UserName { get; set; }

		public UserDetailsViewModel()
		{
		}

		public UserDetailsViewModel(UserModel model)
		{
			UserId = model.Id;
			UserName = model.UserName;
		}
	}
}
