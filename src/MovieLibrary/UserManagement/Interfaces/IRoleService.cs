using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.UserManagement.Interfaces
{
	public interface IRoleService
	{
		Task<string> CreateRole(string roleName, CancellationToken cancellationToken);

		IAsyncEnumerable<RoleModel> GetAllRoles(CancellationToken cancellationToken);

		Task AssignRolePermissions(string roleId, IEnumerable<string> permissions, CancellationToken cancellationToken);

		Task DeleteRole(string roleId, CancellationToken cancellationToken);
	}
}
