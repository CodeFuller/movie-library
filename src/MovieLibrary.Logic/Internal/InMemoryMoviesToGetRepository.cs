using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Internal
{
	public class InMemoryMoviesToGetRepository : IMoviesToGetRepository
	{
		private readonly List<MovieToGetDto> movies = new List<MovieToGetDto>();

		public InMemoryMoviesToGetRepository()
		{
			// Seeding some data to play with.
			movies.Add(new MovieToGetDto
			{
				Id = new MovieId("1"),
				MovieInfo = new MovieInfo
				{
					Title = "Темный рыцарь",
					MovieUri = new Uri("https://www.kinopoisk.ru/film/111543/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_111543.jpg"),
					Director = "Кристофер Нолан",
					Cast = new[] { "Кристиан Бэйл", "Хит Леджер", "Аарон Экхарт", },
					Rating = new MovieRating(8.499M, 463508),
					Duration = TimeSpan.FromMinutes(152),
					Genres = new[] { "фантастика", "боевик", "триллер", },
					Summary = "Бэтмен поднимает ставки в войне с криминалом. С помощью лейтенанта Джима Гордона и прокурора Харви Дента он намерен очистить улицы от преступности, отравляющей город. Сотрудничество оказывается эффективным, но скоро они обнаружат себя посреди хаоса, развязанного восходящим криминальным гением, известным испуганным горожанам под именем Джокер.",
				},
			});

			movies.Add(new MovieToGetDto
			{
				Id = new MovieId("2"),
				MovieInfo = new MovieInfo
				{
					Title = "Гладиатор",
					MovieUri = new Uri("https://www.kinopoisk.ru/film/474/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_474.jpg"),
					Director = "Ридли Скотт",
					Cast = new[] { "Рассел Кроу", "Хоакин Феникс", "Конни Нильсен", },
					Rating = new MovieRating(8.576M, 378263),
					Duration = TimeSpan.FromMinutes(155),
					Genres = new[] { "боевик", "история", "драма", },
					Summary = "В великой Римской империи не было военачальника, равного генералу Максимусу. Непобедимые легионы, которыми командовал этот благородный воин, боготворили его и могли последовать за ним даже в ад.\n\n"
					          +
							  "Но случилось так, что отважный Максимус, готовый сразиться с любым противником в честном бою, оказался бессилен против вероломных придворных интриг. Генерала предали и приговорили к смерти. Чудом избежав гибели, Максимус становится гладиатором.\n\n"
							  +
							  "Быстро снискав себе славу в кровавых поединках, он оказывается в знаменитом римском Колизее, на арене которого он встретится в смертельной схватке со своим заклятым врагом...",
				},
			});

			movies.Add(new MovieToGetDto
			{
				Id = new MovieId("3"),
				MovieInfo = new MovieInfo
				{
					Title = "Большой куш",
					MovieUri = new Uri("https://www.kinopoisk.ru/film/342/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_526.jpg"),
					Director = "Квентин Тарантино",
					Cast = new[] { "Джон Траволта", "Сэмюэл Л. Джексон", "Брюс Уиллис", },
					Rating = new MovieRating(8.619M, 439744),
					Duration = TimeSpan.FromMinutes(154),
					Genres = new[] { "криминал", "комедия", "боевик", },
					Summary = "Двое бандитов Винсент Вега и Джулс Винфилд ведут философские беседы в перерывах между разборками и решением проблем с должниками криминального босса Марселласа Уоллеса.\n\n"
					          +
					          "В первой истории Винсент проводит незабываемый вечер с женой Марселласа Мией. Во второй рассказывается о боксёре Бутче Кулидже, купленном Уоллесом, чтобы сдать бой. В третьей истории Винсент и Джулс по нелепой случайности попадают в неприятности.",
				},
			});
		}

		public Task CreateMovieToGet(MovieToGetDto movieToGet, CancellationToken cancellationToken)
		{
			lock (movies)
			{
				movies.Add(movieToGet);
			}

			return Task.CompletedTask;
		}

		public IAsyncEnumerable<MovieToGetDto> ReadMoviesToGet(CancellationToken cancellationToken)
		{
			List<MovieToGetDto> moviesToReturn;

			lock (movies)
			{
				moviesToReturn = movies.ToList();
			}

			return moviesToReturn.ToAsyncEnumerable();
		}
	}
}
