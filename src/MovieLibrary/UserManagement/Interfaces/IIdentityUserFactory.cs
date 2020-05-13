namespace MovieLibrary.UserManagement.Interfaces
{
	internal interface IIdentityUserFactory<out TUser>
	{
		TUser CreateUser(string userName);
	}
}
