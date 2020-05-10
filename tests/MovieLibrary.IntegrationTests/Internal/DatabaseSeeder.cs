using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Internal;
using MovieLibrary.UserManagement;
using MovieLibrary.UserManagement.Models;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class DatabaseSeeder : IApplicationInitializer
	{
		private readonly ISeedData seedData;

		private readonly IMongoCollection<MovieToGetDocument> moviesToGetDocumentCollection;
		private readonly IMongoCollection<MovieToSeeDocument> moviesToSeeDocumentCollection;
		private readonly IUserService userService;
		private readonly IFakeIdGenerator<ObjectId> fakeIdGenerator;

		public DatabaseSeeder(ISeedData seedData, IMongoCollection<MovieToGetDocument> moviesToGetDocumentCollection,
			IMongoCollection<MovieToSeeDocument> moviesToSeeDocumentCollection, IUserService userService, IFakeIdGenerator<ObjectId> fakeIdGenerator)
		{
			this.seedData = seedData ?? throw new ArgumentNullException(nameof(seedData));
			this.moviesToGetDocumentCollection = moviesToGetDocumentCollection ?? throw new ArgumentNullException(nameof(moviesToGetDocumentCollection));
			this.moviesToSeeDocumentCollection = moviesToSeeDocumentCollection ?? throw new ArgumentNullException(nameof(moviesToSeeDocumentCollection));
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this.fakeIdGenerator = fakeIdGenerator ?? throw new ArgumentNullException(nameof(fakeIdGenerator));
		}

		public async Task Initialize(CancellationToken cancellationToken)
		{
			await SeedMoviesToGet(cancellationToken);
			await SeedMoviesToSee(cancellationToken);
			await SeedUsers(cancellationToken);

			// Default id for new object.
			fakeIdGenerator.SeedIds(new[] { new ObjectId("5eb706d725d7b94ebc88af81") });
		}

		private async Task SeedMoviesToGet(CancellationToken cancellationToken)
		{
			await ClearCollection(moviesToGetDocumentCollection, cancellationToken);
			await InsertDocuments(moviesToGetDocumentCollection, seedData.MoviesToGet, cancellationToken);
		}

		private async Task SeedMoviesToSee(CancellationToken cancellationToken)
		{
			await ClearCollection(moviesToSeeDocumentCollection, cancellationToken);
			await InsertDocuments(moviesToSeeDocumentCollection, seedData.MoviesToSee, cancellationToken);
		}

		private static async Task ClearCollection<TDocument>(IMongoCollection<TDocument> collection, CancellationToken cancellationToken)
		{
			await collection.DeleteManyAsync(new FilterDefinitionBuilder<TDocument>().Empty, cancellationToken);
		}

		private static async Task InsertDocuments<TDocument>(IMongoCollection<TDocument> collection, IEnumerable<TDocument> documents, CancellationToken cancellationToken)
		{
			var insertOptions = new InsertManyOptions
			{
				BypassDocumentValidation = false,
			};

			await collection.InsertManyAsync(documents, insertOptions, cancellationToken);
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

				fakeIdGenerator.SeedIds(new[] { new ObjectId(newUser.Id) });
				var newUserId = await userService.CreateUser(userModel, cancellationToken);
				await userService.AssignUserPermissions(newUserId, newUser.Roles, cancellationToken);
			}
		}
	}
}
