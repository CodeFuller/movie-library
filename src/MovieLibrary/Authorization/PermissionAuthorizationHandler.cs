using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MovieLibrary.Extensions;

namespace MovieLibrary.Authorization
{
	internal class PermissionAuthorizationHandler : AuthorizationHandler<AtLeastOnePermissionRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AtLeastOnePermissionRequirement requirement)
		{
			var user = context.User;

			if (user != null && requirement.Permissions.Any(p => user.HasPermission(p)))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
    }
}
