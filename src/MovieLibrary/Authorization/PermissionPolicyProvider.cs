using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MovieLibrary.Authorization
{
	internal class PermissionPolicyProvider : IAuthorizationPolicyProvider
	{
		private readonly DefaultAuthorizationPolicyProvider fallbackPolicyProvider;

		public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
		{
			fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
		}

		public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
		{
			return fallbackPolicyProvider.GetDefaultPolicyAsync();
		}

		public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
		{
			return fallbackPolicyProvider.GetDefaultPolicyAsync();
		}

		public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
		{
			if (policyName.TryParseApplicationPermissions(out var permissions))
			{
				var policyBuilder = new AuthorizationPolicyBuilder();
				policyBuilder.AddRequirements(new AtLeastOnePermissionRequirement(permissions));

				return Task.FromResult(policyBuilder.Build());
			}

			return fallbackPolicyProvider.GetPolicyAsync(policyName);
		}
	}
}
