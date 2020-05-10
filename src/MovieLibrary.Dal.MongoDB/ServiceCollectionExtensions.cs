using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Dal.MongoDB.Internal;
using MovieLibrary.Dal.MongoDB.Repositories;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.Dal.MongoDB
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMongoDbDal(this IServiceCollection services, string connectionString)
		{
			// It's recommended to use single instance of MongoClient.
			// (http://mongodb.github.io/mongo-csharp-driver/2.0/reference/driver/connecting/#re-use)
			services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));

			// The implementation of IMongoDatabase provided by a MongoClient is thread-safe and is safe to be stored globally or in an IoC container.
			// (http://mongodb.github.io/mongo-csharp-driver/2.0/reference/driver/connecting/)
			var mongoUrl = new MongoUrl(connectionString);
			services.AddSingleton<IMongoDatabase>(sp =>
			{
				var mongoClient = sp.GetService<IMongoClient>();
				return mongoClient.GetDatabase(mongoUrl.DatabaseName);
			});

			services.AddMongoCollection<MovieToGetDocument>("MoviesToGet");
			services.AddMongoCollection<MovieToSeeDocument>("MoviesToSee");

			services.AddSingleton<IIdGenerator<ObjectId>, AutoIdGenerator>();
			services.AddSingleton<IMoviesToGetRepository, MoviesToGetRepository>();
			services.AddSingleton<IMoviesToSeeRepository, MoviesToSeeRepository>();

			return services;
		}

		private static void AddMongoCollection<TDocument>(this IServiceCollection services, string collectionName)
		{
			// The implementation of IMongoCollection<TDocument> ultimately provided by a MongoClient is thread-safe and is safe to be stored globally or in an IoC container.
			// http://mongodb.github.io/mongo-csharp-driver/2.0/reference/driver/connecting/
			services.AddSingleton<IMongoCollection<TDocument>>(sp =>
			{
				var database = sp.GetService<IMongoDatabase>();
				return database.GetCollection<TDocument>(collectionName);
			});
		}
	}
}
