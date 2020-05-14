using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovieLibrary.Internal;
using MovieLibrary.UserManagement.Interfaces;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.Authorization
{
	internal class UsersInitializer : IApplicationInitializer
	{
		private readonly IUserService userService;
		private readonly IRoleService roleService;

		private readonly ILogger<UsersInitializer> logger;

		public UsersInitializer(IUserService userService, IRoleService roleService, ILogger<UsersInitializer> logger)
		{
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Initialize(CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking if users initialization is required ...");

			await EnsureAdministratorRole(cancellationToken);

			await EnsureAdministratorUser(cancellationToken);
		}

		private async Task EnsureAdministratorRole(CancellationToken cancellationToken)
		{
			var administratorRole = await roleService.GetAllRoles(cancellationToken)
				.Where(r => String.Equals(r.RoleName, SecurityConstants.AdministratorRole, StringComparison.Ordinal))
				.FirstOrDefaultAsync(cancellationToken);

			if (administratorRole != null)
			{
				return;
			}

			logger.LogWarning("The role {RoleName} does not exist, creating one ...", SecurityConstants.AdministratorRole);

			await roleService.CreateRole(SecurityConstants.AdministratorRole, cancellationToken);
		}

		private async Task EnsureAdministratorUser(CancellationToken cancellationToken)
		{
			await foreach (var user in userService.GetAllUsers(cancellationToken))
			{
				var userRoles = await userService.GetUserRoles(user.Id, cancellationToken);

				if (userRoles.Any(role => String.Equals(role, SecurityConstants.AdministratorRole, StringComparison.Ordinal)))
				{
					return;
				}
			}

			logger.LogWarning("No user with role {RoleName} exists. Creating {UserName} ...", SecurityConstants.AdministratorRole, SecurityConstants.DefaultAdministratorEmail);

			var newUser = new NewUserModel
			{
				Email = SecurityConstants.DefaultAdministratorEmail,
				Password = SecurityConstants.DefaultAdministratorPassword,
			};

			var userId = await userService.CreateUser(newUser, cancellationToken);
			await userService.AssignUserRoles(userId, new[] { SecurityConstants.AdministratorRole }, cancellationToken);
		}
	}
}
