using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement
{
	public interface IUserService
	{
		Task CreateUser(NewUserModel user, CancellationToken cancellationToken);

		IAsyncEnumerable<UserModel> GetAllUsers(CancellationToken cancellationToken);

		Task<UserDetailsModel> GetUser(string userId, CancellationToken cancellationToken);

		Task AssignUserPermissions(string userId, IEnumerable<string> roles, CancellationToken cancellationToken);

		Task DeleteUser(string userId, CancellationToken cancellationToken);
	}
}
