using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.ViewModels.Users
{
	public class UserViewModel
	{
		public string Id { get; }

		public string UserName { get; }

		public bool CanBeEdited { get; }

		public bool CanBeDeleted { get; }

		public UserViewModel(UserModel model)
		{
			Id = model.Id;
			UserName = model.UserName;
			CanBeEdited = model.CanBeEdited;
			CanBeDeleted = model.CanBeDeleted;
		}
	}
}
