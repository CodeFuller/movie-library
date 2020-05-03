﻿using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.ViewModels
{
	public class UserViewModel
	{
		public string Id { get; }

		public string UserName { get; }

		public UserViewModel(UserModel model)
		{
			Id = model.Id;
			UserName = model.UserName;
		}
	}
}
