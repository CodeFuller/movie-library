using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class FakeRemoteAddressMiddleware
	{
		private readonly RequestDelegate next;

		private readonly string remoteIpAddress;

		public FakeRemoteAddressMiddleware(RequestDelegate next, string remoteIpAddress)
		{
			this.next = next ?? throw new ArgumentNullException(nameof(next));
			this.remoteIpAddress = remoteIpAddress ?? throw new ArgumentNullException(nameof(remoteIpAddress));
		}

		public async Task Invoke(HttpContext context)
		{
			context.Connection.RemoteIpAddress = IPAddress.Parse(remoteIpAddress);

			await this.next(context);
		}
	}
}
