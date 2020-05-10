using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MovieLibrary.Dal.MongoDB.Documents;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal class DefaultSeedData : ISeedData
	{
		public IReadOnlyCollection<MovieToGetDocument> MoviesToGet
		{
			get
			{
				var movieToGet1 = new MovieToGetDocument
				{
					Id = new ObjectId("5eac4f407a15596e90c09d7b"),
					TimestampOfAddingToGetList = new DateTimeOffset(2019, 11, 28, 19, 23, 12, TimeSpan.FromHours(3)),
					MovieInfo = new MovieInfoDocument
					{
						Title = "Криминальное чтиво",
						Year = 1994,
						MovieUri = new Uri("https://www.kinopoisk.ru/film/342/"),
						PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_342.jpg"),
						Directors = new[] { "Квентин Тарантино" },
						Cast = new[] { "Джон Траволта", "Сэмюэл Л. Джексон", "Брюс Уиллис", "Ума Турман", "Винг Реймз", },
						RatingValue = 8.619M,
						RatingVotesNumber = 439744,
						Duration = TimeSpan.FromMinutes(154),
						Genres = new[] { "триллер", "комедия", "криминал", },
						Summary = "Двое бандитов Винсент Вега и Джулс Винфилд ведут философские беседы в перерывах между разборками и решением проблем с должниками криминального босса Марселласа Уоллеса.\n\n"
						          +
						          "В первой истории Винсент проводит незабываемый вечер с женой Марселласа Мией. Во второй рассказывается о боксёре Бутче Кулидже, купленном Уоллесом, чтобы сдать бой. В третьей истории Винсент и Джулс по нелепой случайности попадают в неприятности.",
					},
				};

				var movieToGet2 = new MovieToGetDocument
				{
					Id = new ObjectId("5ead62931969f95b005c1f68"),
					TimestampOfAddingToGetList = new DateTimeOffset(2020, 04, 03, 08, 12, 23, TimeSpan.FromHours(3)),
					MovieInfo = new MovieInfoDocument
					{
						Title = "Movie to get with missing data",
						MovieUri = new Uri("https://www.kinopoisk.ru/film/777/"),
					},
				};

				return new[]
				{
					movieToGet1,
					movieToGet2,
				};
			}
		}

		public IReadOnlyCollection<MovieToSeeDocument> MoviesToSee
		{
			get
			{
				var movieToSee1 = new MovieToSeeDocument
				{
					Id = new ObjectId("5ead62d14be68246b45bba82"),
					TimestampOfAddingToSeeList = new DateTimeOffset(2019, 04, 30, 12, 15, 47, TimeSpan.FromHours(3)),
					MovieInfo = new MovieInfoDocument
					{
						Title = "Гладиатор",
						Year = 2000,
						MovieUri = new Uri("https://www.kinopoisk.ru/film/474/"),
						PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_474.jpg"),
						Directors = new[] { "Ридли Скотт" },
						Cast = new[] { "Рассел Кроу", "Хоакин Феникс", "Конни Нильсен", "Оливер Рид", "Ричард Харрис", },
						RatingValue = 8.576M,
						RatingVotesNumber = 378263,
						Duration = TimeSpan.FromMinutes(155),
						Genres = new[] { "боевик", "история", "драма", "приключения", },
						Summary = "В великой Римской империи не было военачальника, равного генералу Максимусу...",
					},
				};

				var movieToSee2 = new MovieToSeeDocument
				{
					Id = new ObjectId("5ead645f6a24e267d02651d5"),
					TimestampOfAddingToSeeList = new DateTimeOffset(2019, 10, 23, 06, 31, 28, TimeSpan.FromHours(3)),
					MovieInfo = new MovieInfoDocument
					{
						Title = "Movie to see with missing data",
						MovieUri = new Uri("https://www.kinopoisk.ru/film/888/"),
					},
				};

				return new[]
				{
					movieToSee1,
					movieToSee2,
				};
			}
		}
	}
}
