using System;
using System.Collections.Generic;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.IntegrationTests.Internal.Seeding
{
	internal class PagingSeedData : ISeedData
	{
		// We need 10 pages with 2 movies per page.
		private const int MoviesNumber = 10 * 2;

		public IEnumerable<(MovieId Id, MovieInfoModel MovieInfo)> MoviesToGet
		{
			get
			{
				for (var i = 1; i <= MoviesNumber; ++i)
				{
					var movieInfo = new MovieInfoModel
					{
						Title = $"Paging Movie #{i}",
						MovieUri = new Uri($"https://www.kinopoisk.ru/film/{i}/"),
					};

					yield return (new MovieId($"{i:D24}"), movieInfo);
				}
			}
		}

		public IEnumerable<(MovieId Id, MovieInfoModel MovieInfo)> MoviesToSee
		{
			get
			{
				for (var i = 1; i <= MoviesNumber; ++i)
				{
					var movieInfo = new MovieInfoModel
					{
						Title = $"Paging Movie #{i}",
						MovieUri = new Uri($"https://www.kinopoisk.ru/film/{i}/"),
					};

					yield return (new MovieId($"{i:D24}"), movieInfo);
				}
			}
		}

		public IEnumerable<RoleSeedData> Roles => SharedSeedData.ApplicationRoles;

		public IEnumerable<UserSeedData> Users => SharedSeedData.ApplicationUsers;
	}
}
