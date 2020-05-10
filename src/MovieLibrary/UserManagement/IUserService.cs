using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement
{
	public interface IUserService
	{
		Task<string> CreateUser(NewUserModel user, CancellationToken cancellationToken);

		IAsyncEnumerable<UserModel> GetAllUsers(CancellationToken cancellationToken);

		Task<UserDetailsModel> GetUser(string userId, CancellationToken cancellationToken);

		Task AssignUserPermissions(string userId, IEnumerable<string> permissions, CancellationToken cancellationToken);

		Task DeleteUser(string userId, CancellationToken cancellationToken);
	}
}
