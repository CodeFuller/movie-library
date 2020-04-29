using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.IntegrationTests
{
	internal static class MockHelpers
	{
		public static Action<IServiceCollection> StubMovieInfoProviderAndClock(Uri movieUri, MovieInfoModel movieInfo, DateTimeOffset currentTime)
		{
			var servicesSetups = new[]
			{
				StubMovieInfoProvider(movieUri, movieInfo),
				StubClock(currentTime),
			};

			return services =>
			{
				foreach (var serviceSetup in servicesSetups)
				{
					serviceSetup(services);
				}
			};
		}

		private static Action<IServiceCollection> StubMovieInfoProvider(Uri movieUri, MovieInfoModel movieInfo)
		{
			var movieInfoProviderStub = new Mock<IMovieInfoProvider>();
			movieInfoProviderStub.Setup(x => x.GetMovieInfo(movieUri, It.IsAny<CancellationToken>())).ReturnsAsync(movieInfo);

			return services => services.AddSingleton<IMovieInfoProvider>(movieInfoProviderStub.Object);
		}

		public static Action<IServiceCollection> StubClock(DateTimeOffset currentTime)
		{
			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(currentTime);

			return services => services.AddSingleton<IClock>(clockStub.Object);
		}
	}
}
