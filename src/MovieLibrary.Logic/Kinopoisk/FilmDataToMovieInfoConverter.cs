using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MovieLibrary.Logic.Kinopoisk.DataContracts;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal class FilmDataToMovieInfoConverter : IFilmDataToMovieInfoConverter
	{
		public MovieInfoModel Convert(FilmDetailViewDataContract data)
		{
			return new()
			{
				Title = data.NameInRussian,
				Year = data.Year,
				MovieUri = CreateHttpsUrl(data.WebUrl),
				Directors = data.Creators.SelectMany(x => x).Where(x => x.ProfessionKey == "director").Select(x => x.NameInRussian).ToList(),
				Cast = data.Creators.SelectMany(x => x).Where(x => x.ProfessionKey == "actor").Select(x => x.NameInRussian).ToList(),
				Rating = ParseFilmRating(data.RatingData),
				Duration = ParseFilmLength(data.FilmLength),
				Genres = data.Genre.Split(", ").Select(x => x.Trim()).ToList(),
				Summary = data.Description?.Replace("\n\r", "\n\n", StringComparison.Ordinal),
			};
		}

		private static Uri CreateHttpsUrl(string httpUrl)
		{
			var httpsUrl = Regex.Replace(httpUrl, @"^http://(.+)", m => $"https://{m.Groups[1].Value}");
			return new Uri(httpsUrl);
		}

		private static MovieRatingModel ParseFilmRating(FilmRatingDataContract ratingData)
		{
			var ratingValue = ratingData?.Rating;
			if (ratingValue == null)
			{
				return null;
			}

			var votesNumberString = ratingData.RatingVoteCount?.Replace(" ", String.Empty, StringComparison.InvariantCulture);
			var votesNumber = String.IsNullOrEmpty(votesNumberString) ? (int?)null : Int32.Parse(votesNumberString, NumberStyles.None, CultureInfo.InvariantCulture);

			return new MovieRatingModel(ratingValue.Value, votesNumber);
		}

		private static TimeSpan? ParseFilmLength(string filmLength)
		{
			return String.IsNullOrEmpty(filmLength) ? null : TimeSpan.ParseExact(filmLength, "h\\:mm", CultureInfo.InvariantCulture);
		}
	}
}
