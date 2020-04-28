using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MovieLibrary.Internal
{
	public class RolesInitializer : IApplicationInitializer
	{
		private readonly IRoleStore<MongoRole> roleStore;

		private readonly ILogger<RolesInitializer> logger;

		public RolesInitializer(IRoleStore<MongoRole> roleStore, ILogger<RolesInitializer> logger)
		{
			this.roleStore = roleStore ?? throw new ArgumentNullException(nameof(roleStore));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Initialize(CancellationToken cancellationToken)
		{
			foreach (var roleName in Roles.All)
			{
				await CreateRole(roleName, cancellationToken);
			}
		}

		private async Task CreateRole(string roleName, CancellationToken cancellationToken)
		{
			logger.LogInformation("Creating role {RoleName} ...", roleName);

			var role = new MongoRole
			{
				Name = roleName,
				NormalizedName = roleName.ToUpperInvariant(),
				Claims = new List<IdentityRoleClaim<string>>(),
			};

			var result = await roleStore.CreateAsync(role, cancellationToken);

			if (!result.Succeeded)
			{
				logger.LogCritical("Failed to create role {RoleName}. Error(s): {@IdentityErrors}", roleName, result.Errors);
				throw new InvalidOperationException($"Failed to create role {roleName}. Error(s): {result}");
			}
		}
	}
}
