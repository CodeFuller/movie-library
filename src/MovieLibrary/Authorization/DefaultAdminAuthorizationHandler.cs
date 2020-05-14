using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MovieLibrary.Authorization
{
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

			var httpContext = httpContextAccessor.HttpContext;

			var remoteIpAddress = httpContext?.Connection?.RemoteIpAddress;
			if (remoteIpAddress != null && !IPAddress.IsLoopback(remoteIpAddress))
			{
				// Preventing infinite redirects.
				if (httpContext.Request.Path == "/Identity/Account/AccessDenied")
				{
					return Task.CompletedTask;
				}

				logger.LogError("An attempt of unauthorized access via default administrator from address {RemoteIpAddress}", remoteIpAddress.ToString());
				context.Fail();
			}

			return Task.CompletedTask;
		}
	}
}
