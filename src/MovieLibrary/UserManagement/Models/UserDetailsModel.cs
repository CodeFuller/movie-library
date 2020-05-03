using System.Collections.Generic;

namespace MovieLibrary.UserManagement.Models
{
	public class UserDetailsModel
	{
		public string Id { get; set; }

		public string UserName { get; set; }

		public IReadOnlyCollection<string> UserPermissions { get; set; }

		public IReadOnlyCollection<string> AllPermissions { get; set; }
	}
}
