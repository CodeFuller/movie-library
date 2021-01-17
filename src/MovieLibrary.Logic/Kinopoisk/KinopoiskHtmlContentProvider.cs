using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal sealed class KinopoiskHtmlContentProvider : IHtmlContentProvider, IDisposable
	{
		private readonly HttpClient httpClient;

		private readonly ILogger<KinopoiskHtmlContentProvider> logger;

		private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

		private IReadOnlyCollection<string> Cookies { get; set; }

		public KinopoiskHtmlContentProvider(HttpClient httpClient, ILogger<KinopoiskHtmlContentProvider> logger)
		{
			this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<string> GetHtmlPageContent(Uri pageUri, CancellationToken cancellationToken)
		{
			await ProvideCookies(cancellationToken);

			using var response = await ExecuteRequest(pageUri, cancellationToken);
			return await response.Content.ReadAsStringAsync(cancellationToken);
		}

		private async Task ProvideCookies(CancellationToken cancellationToken)
		{
			if (Cookies != null)
			{
				return;
			}

			await semaphore.WaitAsync(cancellationToken);

			// Checking for null again, since concurrent thread could load them while we were waiting for semaphore.
#pragma warning disable CA1508 // Avoid dead conditional code
			if (Cookies != null)
#pragma warning restore CA1508 // Avoid dead conditional code
			{
				return;
			}

			await LoadCookies(cancellationToken);
		}

		private async Task LoadCookies(CancellationToken cancellationToken)
		{
			logger.LogInformation("Loading Kinopoisk cookies ...");

			using var response = await ExecuteRequest(new Uri("https://www.kinopoisk.ru/"), cancellationToken);

			Cookies = response.Headers
				.Where(header => header.Key == "Set-Cookie")
				.SelectMany(header => header.Value)
				.ToList();

			logger.LogInformation("Loaded Kinopoisk cookies successfully");
		}

		private async Task<HttpResponseMessage> ExecuteRequest(Uri uri, CancellationToken cancellationToken)
		{
			using var request = CreateRequest(uri);
			return await ExecuteRequest(request, cancellationToken);
		}

		private HttpRequestMessage CreateRequest(Uri uri)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, uri);

			// Imitating headers sent by IE browser.
			request.Headers.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
			request.Headers.Add("Accept-Language", "en-US");
			request.Headers.Add("Accept-Encoding", "gzip, deflate");
			request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");

			// Connection: Keep-Alive
			request.Headers.ConnectionClose = false;

			request.Headers.Add("Referer", "https://www.kinopoisk.ru/");

			foreach (var cookie in Cookies ?? Enumerable.Empty<string>())
			{
				request.Headers.Add("Cookie", cookie);
			}

			return request;
		}

		private async Task<HttpResponseMessage> ExecuteRequest(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response = await httpClient.SendAsync(request, cancellationToken);

			if (!response.IsSuccessStatusCode)
			{
				logger.LogWarning("HTTP request to {HttpRequestUri} has failed with code {HttpRequestStatusCode}", request.RequestUri, response.StatusCode);
				throw new InvalidOperationException($"Request to {request.RequestUri} failed with code {response.StatusCode}");
			}

			return response;
		}

		public void Dispose()
		{
			semaphore?.Dispose();
		}
	}
}
