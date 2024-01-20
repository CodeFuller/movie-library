using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MovieLibrary.Logic.Kinopoisk.DataContracts;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal class FilmDataToMovieInfoConverter : IFilmDataToMovieInfoConverter
	{
		private static readonly string[] NewLineSeparators = { "\r\n\r\n", "\n\n", "\n\r" };

		public MovieInfoModel Convert(FilmDetailViewDataContract data)
		{
			return new()
			{
				Title = data.NameInRussian,
				Year = data.Year,
				MovieUri = CreateHttpsUrl(data.WebUrl),
				Directors = GetCreators(data, "director"),
				Cast = GetCreators(data, "actor"),
				Rating = ParseFilmRating(data.RatingData),
				Duration = ParseFilmLength(data.FilmLength),
				Genres = data.Genre.Split(", ").Select(x => x.Trim()).ToList(),
				Countries = data.Country.Split(", ").Select(x => x.Trim()).ToList(),

				// Sequence "\n\r" is quite strange but it happens for some movies, e.g. https://www.kinopoisk.ru/film/342/
				SummaryParagraphs = data.Description?.Split(NewLineSeparators, StringSplitOptions.None),
			};
		}

		private static Uri CreateHttpsUrl(string httpUrl)
		{
			var httpsUrl = Regex.Replace(httpUrl, @"^http://(.+)", m => $"https://{m.Groups[1].Value}");
			return new Uri(httpsUrl);
		}

		private static List<string> GetCreators(FilmDetailViewDataContract data, string professionKey)
		{
			return data.Creators
				.SelectMany(x => x)
				.Where(x => x.ProfessionKey == professionKey)
				.Select(x => x.Name)
				.Where(x => x != null)
				.ToList();
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
