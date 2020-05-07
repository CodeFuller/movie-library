using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.Logic.IntegrationTests
{
	internal static class MockHelpers
	{
		public static Action<IServiceCollection> StubClock(DateTimeOffset currentTime)
		{
			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(currentTime);

			return services => services.AddSingleton<IClock>(clockStub.Object);
		}
	}
}
