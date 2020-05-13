using AspNetCore.Identity.Mongo.Model;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MovieLibrary.UserManagement.Interfaces;

namespace MovieLibrary.UserManagement
{
	internal static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddUserManagement(this IServiceCollection services)
		{
			services.AddTransient<IUserService, UserService<MongoUser, ObjectId>>();
			services.AddTransient<IRoleService, RoleService<MongoRole, ObjectId>>();
			services.AddTransient(typeof(IUserManager<,>), typeof(UserManagerWrapper<,>));
			services.AddTransient(typeof(IRoleManager<,>), typeof(RoleManagerWrapper<,>));

			services.AddSingleton<IIdentityUserFactory<MongoUser>, MongoIdentityUserFactory>();
			services.AddSingleton<IIdentityRoleFactory<MongoRole>, MongoIdentityRoleFactory>();

			return services;
		}
	}
}
