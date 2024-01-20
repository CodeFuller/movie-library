using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Kinopoisk.DataContracts;
using MovieLibrary.Logic.Models;
using Newtonsoft.Json;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal sealed class KinopoiskApiMovieInfoProvider : IMovieInfoProvider, IDisposable
	{
		private static Uri BaseKinopoiskApiUri => new("https://ma.kinopoisk.ru/ios/5.0.0/");

		// We concatenate salt from two strings to avoid search via search engines by full value.
		private const string KinopoiskApiSalt = "IDATe" + "vHDS7";

		private readonly HttpClient httpClient;

		private readonly IFilmDataToMovieInfoConverter filmDataConverter;

		private readonly ILogger<KinopoiskApiMovieInfoProvider> logger;

		private readonly MD5 md5;

		public KinopoiskApiMovieInfoProvider(HttpClient httpClient, IFilmDataToMovieInfoConverter filmDataConverter, ILogger<KinopoiskApiMovieInfoProvider> logger)
		{
			this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			this.filmDataConverter = filmDataConverter ?? throw new ArgumentNullException(nameof(filmDataConverter));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
			this.md5 = MD5.Create();
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
		}

		public async Task<MovieInfoModel> GetMovieInfo(Uri movieUri, CancellationToken cancellationToken)
		{
			logger.LogInformation("Getting movie info for URL {MovieUri}", movieUri);

			if (!String.Equals(movieUri.Host, "www.kinopoisk.ru", StringComparison.OrdinalIgnoreCase))
			{
				throw new NotSupportedException($"Loading movie info from {movieUri} is not supported");
			}

			var filmId = ParseFilmIdFromUri(movieUri);
			using var requestMessage = PrepareRequestForFilmDetails(filmId);

			using var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
			var jsonData = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

			var filmDetailsResponse = JsonConvert.DeserializeObject<GetKPFilmDetailViewResponse>(jsonData);

			var movieInfo = filmDataConverter.Convert(filmDetailsResponse.Data);
			movieInfo.PosterUri = await GetFilmPosterUri(filmId, cancellationToken);

			return movieInfo;
		}

		private static string ParseFilmIdFromUri(Uri movieUri)
		{
			var regex = new Regex(@"^https?://www\.kinopoisk\.ru/film/(\d+)/?$");
			var match = regex.Match(movieUri.OriginalString);
			if (!match.Success)
			{
				throw new InvalidOperationException($"Failed to parse film id from kinopoisk URL '{movieUri.OriginalString}'");
			}

			return match.Groups[1].Value;
		}

		private HttpRequestMessage PrepareRequestForFilmDetails(string filmId)
		{
			var clientId = Guid.NewGuid().ToString("N");
			var uuid = Guid.NewGuid().ToString("N");
			var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString("D", CultureInfo.InvariantCulture);

			var relativePath = $"getKPFilmDetailView?still_limit=9&filmID={filmId}&uuid={uuid}";

			var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(BaseKinopoiskApiUri, relativePath));
			requestMessage.Headers.Add("Accept-encoding", "gzip");
			requestMessage.Headers.Add("Accept", "application/json");
			requestMessage.Headers.Add("Image-Scale", "3");
			requestMessage.Headers.Add("device", "android");
			requestMessage.Headers.Add("ClientId", clientId);
			requestMessage.Headers.Add("countryID", "2");
			requestMessage.Headers.Add("cityID", "1");
			requestMessage.Headers.Add("Android-Api-Version", "23");
			requestMessage.Headers.Add("clientDate", DateTime.Now.ToString("HH:mm dd.MM.yyyy", CultureInfo.InvariantCulture));
			requestMessage.Headers.Add("X-TIMESTAMP", timestamp);
			requestMessage.Headers.Add("X-SIGNATURE", GetRequestSignature(relativePath, timestamp));

			return requestMessage;
		}

		private string GetRequestSignature(string requestPath, string timestamp)
		{
			return GetMD5ForString(requestPath + timestamp + KinopoiskApiSalt);
		}

		private string GetMD5ForString(string data)
		{
			var inputBytes = Encoding.ASCII.GetBytes(data);
			var hashBytes = md5.ComputeHash(inputBytes);

#pragma warning disable CA1308 // Normalize strings to uppercase
			return Convert.ToHexString(hashBytes).ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
		}

		private async Task<Uri> GetFilmPosterUri(string filmId, CancellationToken cancellationToken)
		{
			using var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"https://st.kp.yandex.net/images/film_big/{filmId}.jpg"));
			requestMessage.Headers.Add("Referer", $"https://www.kinopoisk.ru/film/{filmId}/");

			var httResponse = await httpClient.SendAsync(requestMessage, cancellationToken);
			if (httResponse.StatusCode != HttpStatusCode.Redirect)
			{
				throw new InvalidOperationException("Failed to get redirect response for movie poster");
			}

			var posterUri = httResponse.Headers.Location;
			if (posterUri == null)
			{
				throw new InvalidOperationException("Location of poster URL is missing");
			}

			if (posterUri == new Uri("https://st.kp.yandex.net/images/no-poster.gif"))
			{
				return null;
			}

			return posterUri;
		}

		public void Dispose()
		{
			md5?.Dispose();
		}
	}
}
