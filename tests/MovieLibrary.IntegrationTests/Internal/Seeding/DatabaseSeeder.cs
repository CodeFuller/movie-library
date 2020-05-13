﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		public DatabaseSeeder(ISeedData seedData, IMoviesToGetService moviesToGetService, IMoviesToSeeService moviesToSeeService,
			IUserService userService, IRoleService roleService, IIdGeneratorQueue idGeneratorQueue)
		{
			this.seedData = seedData ?? throw new ArgumentNullException(nameof(seedData));
			this.moviesToGetService = moviesToGetService ?? throw new ArgumentNullException(nameof(moviesToGetService));
			this.moviesToSeeService = moviesToSeeService ?? throw new ArgumentNullException(nameof(moviesToSeeService));
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
			this.idGeneratorQueue = idGeneratorQueue ?? throw new ArgumentNullException(nameof(idGeneratorQueue));
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
			var oldMovies = moviesToGetService.GetAllMovies().ToList();
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
			var oldMovies = moviesToSeeService.GetAllMovies().ToList();
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
			var prevRoles = await roleService.GetAllRoles(cancellationToken).ToListAsync(cancellationToken);
			foreach (var prevRole in prevRoles)
			{
				await roleService.DeleteRole(prevRole.Id, cancellationToken);
			}

			foreach (var newRole in seedData.Roles)
			{
				idGeneratorQueue.EnqueueId(newRole.Id);
				var roleId = await roleService.CreateRole(newRole.RoleName, cancellationToken);
				await roleService.AssignRolePermissions(roleId, newRole.Permissions, cancellationToken);
			}
		}

		private async Task SeedUsers(CancellationToken cancellationToken)
		{
			var prevUsers = await userService.GetAllUsers(cancellationToken).ToListAsync(cancellationToken);
			foreach (var prevUser in prevUsers)
			{
				await userService.DeleteUser(prevUser.Id, cancellationToken);
			}

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