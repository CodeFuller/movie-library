using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MovieLibrary.Dal.MongoDB.Documents;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class PagingSeedData : ISeedData
	{
		// We need 10 pages with 2 movies per page.
		private const int MoviesNumber = 10 * 2;

		public IEnumerable<MovieToGetDocument> MoviesToGet
		{
			get
			{
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

					yield return movie;
				}
			}
		}

		public IEnumerable<MovieToSeeDocument> MoviesToSee
		{
			get
			{
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

					yield return movie;
				}
			}
		}

		public IEnumerable<UserSeedData> Users => Enumerable.Empty<UserSeedData>();
	}
}
