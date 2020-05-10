using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MovieLibrary.Dal.MongoDB.Documents;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class PagingSeedData : ISeedData
	{
		// We need 10 pages with 2 movies per page.
		private const int MoviesNumber = 10 * 2;

		public IReadOnlyCollection<MovieToGetDocument> MoviesToGet
		{
			get
			{
				var movies = new List<MovieToGetDocument>(MoviesNumber);

				for (var i = 1; i <= MoviesNumber; ++i)
				{
					var movie = new MovieToGetDocument
					{
						Id = new ObjectId($"{i:D24}"),
						TimestampOfAddingToGetList = new DateTimeOffset(2020, 05, 10, 10, 54, i, TimeSpan.FromHours(3)),
						MovieInfo = new MovieInfoDocument
						{
							Title = $"Paging Movie #{i}",
							MovieUri = new Uri($"https://www.kinopoisk.ru/film/{i}/"),
						},
					};

					movies.Add(movie);
				}

				return movies;
			}
		}

		public IReadOnlyCollection<MovieToSeeDocument> MoviesToSee
		{
			get
			{
				var movies = new List<MovieToSeeDocument>(MoviesNumber);

				for (var i = 1; i <= MoviesNumber; ++i)
				{
					var movie = new MovieToSeeDocument
					{
						Id = new ObjectId($"{i:D24}"),
						TimestampOfAddingToSeeList = new DateTimeOffset(2020, 05, 10, 10, 54, i, TimeSpan.FromHours(3)),
						MovieInfo = new MovieInfoDocument
						{
							Title = $"Paging Movie #{i}",
							MovieUri = new Uri($"https://www.kinopoisk.ru/film/{i}/"),
						},
					};

					movies.Add(movie);
				}

				return movies;
			}
		}
	}
}
