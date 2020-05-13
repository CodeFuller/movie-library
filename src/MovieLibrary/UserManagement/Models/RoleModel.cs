using System.Collections.Generic;

namespace MovieLibrary.UserManagement.Models
{
	public class RoleModel
	{
		public string Id { get; set; }

		public string RoleName { get; set; }

		public IReadOnlyCollection<string> Permissions { get; set; }
	}
}
