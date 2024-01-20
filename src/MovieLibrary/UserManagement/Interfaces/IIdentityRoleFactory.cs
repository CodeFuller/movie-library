namespace MovieLibrary.UserManagement.Interfaces
{
	internal interface IIdentityRoleFactory<out TRole>
	{
		TRole CreateRole(string roleName);
	}
}
