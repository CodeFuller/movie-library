using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.ViewModels.Roles
{
	public class RoleViewModel
	{
		public string Id { get; }

		public string RoleName { get; }

		public bool ReadOnly { get; }

		public RoleViewModel(RoleModel model)
		{
			Id = model.Id;
			RoleName = model.RoleName;
			ReadOnly = model.ReadOnly;
		}
	}
}
