using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MovieLibrary.Authorization
{
	// TBD: Cover with UT
	internal class DefaultAdminAuthorizationHandler : IAuthorizationHandler
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		private readonly ILogger<DefaultAdminAuthorizationHandler> logger;

		public DefaultAdminAuthorizationHandler(IHttpContextAccessor httpContextAccessor, ILogger<DefaultAdminAuthorizationHandler> logger)
		{
			this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Task HandleAsync(AuthorizationHandlerContext context)
		{
			var user = context.User;
			if (user == null)
			{
				return Task.CompletedTask;
			}

			if (!String.Equals(user.Identity.Name, SecurityConstants.DefaultAdministratorEmail, StringComparison.Ordinal))
			{
				return Task.CompletedTask;
			}

			var remoteIpAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;
			if (remoteIpAddress != null && !IPAddress.IsLoopback(remoteIpAddress))
			{
				logger.LogError("An attempt of unauthorized access via default administrator from address {RemoteIpAddress}", remoteIpAddress.ToString());
				context.Fail();
			}

			return Task.CompletedTask;
		}
	}
}
