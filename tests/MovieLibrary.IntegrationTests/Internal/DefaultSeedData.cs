using System;
using System.Collections.Generic;
using System.Linq;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class DefaultSeedData : ISeedData
	{
		public IEnumerable<(MovieId id, MovieInfoModel movieInfo)> MoviesToGet
		{
			get
			{
				var movieInfo1 = new MovieInfoModel
				{
					Title = "Криминальное чтиво",
					Year = 1994,
					MovieUri = new Uri("https://www.kinopoisk.ru/film/342/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_342.jpg"),
					Directors = new[] { "Квентин Тарантино" },
					Cast = new[] { "Джон Траволта", "Сэмюэл Л. Джексон", "Брюс Уиллис", "Ума Турман", "Винг Реймз", },
					Rating = new MovieRatingModel(8.619M, 439744),
					Duration = TimeSpan.FromMinutes(154),
					Genres = new[] { "триллер", "комедия", "криминал", },
					Summary = "Двое бандитов Винсент Вега и Джулс Винфилд ведут философские беседы в перерывах между разборками и решением проблем с должниками криминального босса Марселласа Уоллеса.\n\n"
					          +
					          "В первой истории Винсент проводит незабываемый вечер с женой Марселласа Мией. Во второй рассказывается о боксёре Бутче Кулидже, купленном Уоллесом, чтобы сдать бой. В третьей истории Винсент и Джулс по нелепой случайности попадают в неприятности.",
				};

				yield return (new MovieId("5eac4f407a15596e90c09d7b"), movieInfo1);

				var movieInfo2 = new MovieInfoModel
				{
					Title = "Movie to get with missing data",
					MovieUri = new Uri("https://www.kinopoisk.ru/film/777/"),
				};

				yield return (new MovieId("5ead62931969f95b005c1f68"), movieInfo2);
			}
		}

		public IEnumerable<(MovieId id, MovieInfoModel movieInfo)> MoviesToSee
		{
			get
			{
				var movieInfo1 = new MovieInfoModel
				{
					Title = "Гладиатор",
					Year = 2000,
					MovieUri = new Uri("https://www.kinopoisk.ru/film/474/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_474.jpg"),
					Directors = new[] { "Ридли Скотт" },
					Cast = new[] { "Рассел Кроу", "Хоакин Феникс", "Конни Нильсен", "Оливер Рид", "Ричард Харрис", },
					Rating = new MovieRatingModel(8.576M, 378263),
					Duration = TimeSpan.FromMinutes(155),
					Genres = new[] { "боевик", "история", "драма", "приключения", },
					Summary = "В великой Римской империи не было военачальника, равного генералу Максимусу...",
				};

				yield return (new MovieId("5ead62d14be68246b45bba82"), movieInfo1);

				var movieInfo2 = new MovieInfoModel
				{
					Title = "Movie to see with missing data",
					MovieUri = new Uri("https://www.kinopoisk.ru/film/888/"),
				};

				yield return (new MovieId("5ead645f6a24e267d02651d5"), movieInfo2);
			}
		}

		public IEnumerable<UserSeedData> Users
		{
			get
			{
				yield return new UserSeedData
				{
					Id = "5eb7eb9e1fdada19f4eb59b0",
					Email = "SomeAdministrator@test.com",
					Password = "Qwerty123!",
					Roles = UserRoles.AdministratorRoles.ToList(),
				};

				yield return new UserSeedData
				{
					Id = "5eb7eb9f1fdada19f4eb59b1",
					Email = "SomeLimitedUser@test.com",
					Password = "Qwerty321!",
					Roles = UserRoles.LimitedUserRoles.ToList(),
				};
			}
		}
	}
}
