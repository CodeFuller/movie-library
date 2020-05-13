using AspNetCore.Identity.Mongo.Model;
using MovieLibrary.UserManagement.Interfaces;

namespace MovieLibrary.UserManagement
{
	internal class MongoIdentityRoleFactory : IIdentityRoleFactory<MongoRole>
	{
		public MongoRole CreateRole(string roleName)
		{
			return new MongoRole(roleName);
		}
	}
}
