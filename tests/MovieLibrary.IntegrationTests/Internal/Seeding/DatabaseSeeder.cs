using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovieLibrary.Internal;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.UserManagement.Interfaces;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal class DatabaseSeeder : IApplicationInitializer
	{
		private readonly ISeedData seedData;

		private readonly IMoviesToGetService moviesToGetService;
		private readonly IMoviesToSeeService moviesToSeeService;

		private readonly IUserService userService;
		private readonly IRoleService roleService;

		private readonly IIdGeneratorQueue idGeneratorQueue;

		private readonly ILogger<DatabaseSeeder> logger;

		public DatabaseSeeder(ISeedData seedData, IMoviesToGetService moviesToGetService, IMoviesToSeeService moviesToSeeService,
			IUserService userService, IRoleService roleService, IIdGeneratorQueue idGeneratorQueue, ILogger<DatabaseSeeder> logger)
		{
			this.seedData = seedData ?? throw new ArgumentNullException(nameof(seedData));
			this.moviesToGetService = moviesToGetService ?? throw new ArgumentNullException(nameof(moviesToGetService));
			this.moviesToSeeService = moviesToSeeService ?? throw new ArgumentNullException(nameof(moviesToSeeService));
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
			this.idGeneratorQueue = idGeneratorQueue ?? throw new ArgumentNullException(nameof(idGeneratorQueue));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Initialize(CancellationToken cancellationToken)
		{
			await SeedMoviesToGet(cancellationToken);
			await SeedMoviesToSee(cancellationToken);
			await SeedRoles(cancellationToken);
			await SeedUsers(cancellationToken);

			if (seedData.Users.Any())
			{
				// Default id for new movie.
				idGeneratorQueue.EnqueueId("5eb706d725d7b94ebc88af81");
			}
			else
			{
				// Ids for default administrator role and administrator user.
				idGeneratorQueue.EnqueueIds(new[] { "5eb706d725d7b94ebc88af82", "5eb706d725d7b94ebc88af83" });
			}
		}

		private async Task SeedMoviesToGet(CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding movies to get ...");

			var oldMovies = moviesToGetService.GetAllMovies().ToList();

			logger.LogInformation("Deleting movies to get: {DeletedMoviesToGet}", oldMovies.Select(x => x.Id.Value));
			foreach (var oldMovie in oldMovies)
			{
				await moviesToGetService.DeleteMovie(oldMovie.Id, cancellationToken);
			}

			foreach (var (id, movieInfo) in seedData.MoviesToGet)
			{
				idGeneratorQueue.EnqueueId(id.Value);
				await moviesToGetService.AddMovie(movieInfo, cancellationToken);
			}
		}

		private async Task SeedMoviesToSee(CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding movies to see ...");

			var oldMovies = moviesToSeeService.GetAllMovies().ToList();

			logger.LogInformation("Deleting movies to see: {DeletedMoviesToSee}", oldMovies.Select(x => x.Id.Value));
			foreach (var oldMovie in oldMovies)
			{
				await moviesToSeeService.DeleteMovie(oldMovie.Id, cancellationToken);
			}

			foreach (var (id, movieInfo) in seedData.MoviesToSee)
			{
				idGeneratorQueue.EnqueueId(id.Value);
				await moviesToSeeService.AddMovie(movieInfo, cancellationToken);
			}
		}

		private async Task SeedRoles(CancellationToken cancellationToken)
		{
			await roleService.Clear(cancellationToken);

			foreach (var newRole in seedData.Roles)
			{
				idGeneratorQueue.EnqueueId(newRole.Id);
				var roleId = await roleService.CreateRole(newRole.RoleName, cancellationToken);

				if (newRole.Permissions.Any())
				{
					await roleService.AssignRolePermissions(roleId, newRole.Permissions, cancellationToken);
				}
			}
		}

		private async Task SeedUsers(CancellationToken cancellationToken)
		{
			await userService.Clear(cancellationToken);

			foreach (var newUser in seedData.Users)
			{
				var userModel = new NewUserModel
				{
					Email = newUser.Email,
					Password = newUser.Password,
				};

				idGeneratorQueue.EnqueueId(newUser.Id);
				var newUserId = await userService.CreateUser(userModel, cancellationToken);
				await userService.AssignUserRoles(newUserId, newUser.Roles, cancellationToken);
			}
		}
	}
}
