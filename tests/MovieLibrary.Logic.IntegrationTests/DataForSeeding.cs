using System;
using System.Collections.Generic;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.IntegrationTests
{
	internal static class DataForSeeding
	{
		public static MovieToGetModel MovieToGet1 => new()
		{
			TimestampOfAddingToGetList = new DateTimeOffset(2019, 11, 28, 19, 23, 12, TimeSpan.FromHours(3)),
			MovieInfo = new MovieInfoModel
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
				Countries = new[] { "США", },
				SummaryParagraphs = new[]
				{
					"Двое бандитов Винсент Вега и Джулс Винфилд ведут философские беседы в перерывах между разборками и решением проблем с должниками криминального босса Марселласа Уоллеса.",
					"В первой истории Винсент проводит незабываемый вечер с женой Марселласа Мией. Во второй рассказывается о боксёре Бутче Кулидже, купленном Уоллесом, чтобы сдать бой. В третьей истории Винсент и Джулс по нелепой случайности попадают в неприятности.",
				},
			},
			Reference = "Известный фильм",
		};

		public static MovieToGetModel MovieToGet2 => new()
		{
			TimestampOfAddingToGetList = new DateTimeOffset(2020, 04, 03, 08, 12, 23, TimeSpan.FromHours(3)),
			MovieInfo = new MovieInfoModel
			{
				Title = "Movie to get with missing data",
				MovieUri = new Uri("https://www.kinopoisk.ru/film/777/"),
			},
			Reference = "http://www.example.com/",
		};

		public static MovieToGetModel MovieToGet3 => new()
		{
			TimestampOfAddingToGetList = new DateTimeOffset(2020, 05, 08, 08, 17, 54, TimeSpan.FromHours(3)),
			MovieInfo = new MovieInfoModel
			{
				Title = "Movie to get with missing data 2",
				MovieUri = new Uri("https://www.kinopoisk.ru/film/778/"),
			},
			Reference = null,
		};

		public static MovieToGetModel MovieToGet4 => new()
		{
			TimestampOfAddingToGetList = new DateTimeOffset(2020, 05, 08, 08, 19, 37, TimeSpan.FromHours(3)),
			MovieInfo = new MovieInfoModel
			{
				Title = "Movie to get with missing data 3",
				MovieUri = new Uri("https://www.kinopoisk.ru/film/779/"),
			},
			Reference = null,
		};

		public static IReadOnlyCollection<MovieToGetModel> MoviesToGet { get; } = new[] { MovieToGet1, MovieToGet2, MovieToGet3, MovieToGet4, };

		public static MovieToSeeModel MovieToSee1 => new()
		{
			TimestampOfAddingToSeeList = new DateTimeOffset(2019, 04, 30, 12, 15, 47, TimeSpan.FromHours(3)),
			MovieInfo = new MovieInfoModel
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
				Countries = new[] { "США", "Великобритания", "Мальта", "Марокко", },
				SummaryParagraphs = new[]
				{
					"В великой Римской империи не было военачальника, равного генералу Максимусу...",
				},
			},
			Reference = "Классика",
		};

		public static MovieToSeeModel MovieToSee2 => new()
		{
			TimestampOfAddingToSeeList = new DateTimeOffset(2019, 10, 23, 06, 31, 28, TimeSpan.FromHours(3)),
			MovieInfo = new MovieInfoModel
			{
				Title = "Movie to see with missing data",
				MovieUri = new Uri("https://www.kinopoisk.ru/film/888/"),
			},
			Reference = "http://www.example.com/",
		};

		public static MovieToSeeModel MovieToSee3 => new()
		{
			TimestampOfAddingToSeeList = new DateTimeOffset(2020, 05, 08, 08, 33, 51, TimeSpan.FromHours(3)),
			MovieInfo = new MovieInfoModel
			{
				Title = "Movie to see with missing data 2",
				MovieUri = new Uri("https://www.kinopoisk.ru/film/889/"),
			},
			Reference = null,
		};

		public static MovieToSeeModel MovieToSee4 => new()
		{
			TimestampOfAddingToSeeList = new DateTimeOffset(2020, 05, 08, 08, 34, 07, TimeSpan.FromHours(3)),
			MovieInfo = new MovieInfoModel
			{
				Title = "Movie to see with missing data 3",
				MovieUri = new Uri("https://www.kinopoisk.ru/film/890/"),
			},
			Reference = null,
		};

		public static IReadOnlyCollection<MovieToSeeModel> MoviesToSee { get; } = new[] { MovieToSee1, MovieToSee2, MovieToSee3, MovieToSee4, };
	}
}
