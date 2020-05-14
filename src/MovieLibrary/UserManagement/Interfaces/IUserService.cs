using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.Interfaces
{
	public interface IUserService
	{
		Task<string> CreateUser(NewUserModel user, CancellationToken cancellationToken);

		Task CreateDefaultAdministrator(CancellationToken cancellationToken);

		IAsyncEnumerable<UserModel> GetAllUsers(CancellationToken cancellationToken);

		Task<UserModel> GetUser(string userId, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<string>> GetUserRoles(string userId, CancellationToken cancellationToken);

		Task AssignUserRoles(string userId, IEnumerable<string> roles, CancellationToken cancellationToken);

		Task DeleteUser(string userId, CancellationToken cancellationToken);
	}
}
