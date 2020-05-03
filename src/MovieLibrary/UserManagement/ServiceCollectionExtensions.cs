using AspNetCore.Identity.Mongo.Model;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;

namespace MovieLibrary.UserManagement
{
	internal static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddUserManagement(this IServiceCollection services)
		{
			services.AddTransient<IUserService, UserService<MongoUser, MongoRole, ObjectId>>();
			services.AddTransient(typeof(IUserManager<,>), typeof(UserManagerWrapper<,>));
			services.AddTransient(typeof(IRoleManager<,>), typeof(RoleManagerWrapper<,>));

			return services;
		}
	}
}
