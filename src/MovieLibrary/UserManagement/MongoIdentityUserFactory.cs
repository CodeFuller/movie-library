using AspNetCore.Identity.Mongo.Model;
using MovieLibrary.UserManagement.Interfaces;

namespace MovieLibrary.UserManagement
{
	internal class MongoIdentityUserFactory : IIdentityUserFactory<MongoUser>
	{
		public MongoUser CreateUser(string userName)
		{
			return new MongoUser(userName);
		}
	}
}
