﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieLibrary.Logic.Dto;
using MovieLibrary.Logic.Exceptions;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;
using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Internal
{
	internal class InMemoryMoviesToGetRepository : IMoviesToGetRepository
	{
		private readonly List<MovieToGetDto> movies = new List<MovieToGetDto>();

		public InMemoryMoviesToGetRepository()
		{
			// Seeding some data to play with.
			movies.Add(new MovieToGetDto
			{
				Id = new MovieId("1"),
				TimestampOfAddingToGetList = new DateTimeOffset(2020, 04, 26, 12, 55, 35, TimeSpan.FromHours(3)),
				MovieInfo = new MovieInfo
				{
					Title = "Темный рыцарь",
					Year = 2008,
					MovieUri = new Uri("https://www.kinopoisk.ru/film/111543/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_111543.jpg"),
					Directors = new[] { "Кристофер Нолан" },
					Cast = new[] { "Кристиан Бэйл", "Хит Леджер", "Аарон Экхарт", "Мэгги Джилленхол", "Гари Олдман", },
					Rating = new MovieRating(8.499M, 463508),
					Duration = TimeSpan.FromMinutes(152),
					Genres = new[] { "фантастика", "боевик", "триллер", "криминал", "драма", },
					Summary = "Бэтмен поднимает ставки в войне с криминалом. С помощью лейтенанта Джима Гордона и прокурора Харви Дента он намерен очистить улицы от преступности, отравляющей город. Сотрудничество оказывается эффективным, но скоро они обнаружат себя посреди хаоса, развязанного восходящим криминальным гением, известным испуганным горожанам под именем Джокер.",
				},
			});

			movies.Add(new MovieToGetDto
			{
				Id = new MovieId("2"),
				TimestampOfAddingToGetList = new DateTimeOffset(2019, 11, 28, 19, 23, 12, TimeSpan.FromHours(3)),
				MovieInfo = new MovieInfo
				{
					Title = "Гладиатор",
					Year = 2000,
					MovieUri = new Uri("https://www.kinopoisk.ru/film/474/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_474.jpg"),
					Directors = new[] { "Ридли Скотт" },
					Cast = new[] { "Рассел Кроу", "Хоакин Феникс", "Конни Нильсен", "Оливер Рид", "Ричард Харрис", },
					Rating = new MovieRating(8.576M, 378263),
					Duration = TimeSpan.FromMinutes(155),
					Genres = new[] { "боевик", "история", "драма", "приключения", },
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
				TimestampOfAddingToGetList = new DateTimeOffset(2020, 04, 03, 08, 12, 23, TimeSpan.FromHours(3)),
				MovieInfo = new MovieInfo
				{
					Title = "Криминальное чтиво",
					Year = 1994,
					MovieUri = new Uri("https://www.kinopoisk.ru/film/342/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_342.jpg"),
					Directors = new[] { "Квентин Тарантино" },
					Cast = new[] { "Джон Траволта", "Сэмюэл Л. Джексон", "Брюс Уиллис", "Ума Турман", "Винг Реймз", },
					Rating = new MovieRating(8.619M, 439744),
					Duration = TimeSpan.FromMinutes(154),
					Genres = new[] { "триллер", "комедия", "криминал", },
					Summary = "Двое бандитов Винсент Вега и Джулс Винфилд ведут философские беседы в перерывах между разборками и решением проблем с должниками криминального босса Марселласа Уоллеса.\n\n"
					          +
					          "В первой истории Винсент проводит незабываемый вечер с женой Марселласа Мией. Во второй рассказывается о боксёре Бутче Кулидже, купленном Уоллесом, чтобы сдать бой. В третьей истории Винсент и Джулс по нелепой случайности попадают в неприятности.",
				},
			});

			movies.Add(new MovieToGetDto
			{
				Id = new MovieId("4"),
				TimestampOfAddingToGetList = new DateTimeOffset(2020, 04, 26, 13, 06, 22, TimeSpan.FromHours(3)),
				MovieInfo = new MovieInfo
				{
					Title = "Film with no data",
					MovieUri = new Uri("https://www.kinopoisk.ru/film/1/"),
				},
			});
		}

		public Task AddMovie(MovieToGetDto movie, CancellationToken cancellationToken)
		{
			lock (movies)
			{
				movies.Add(movie);
			}

			return Task.CompletedTask;
		}

		public IAsyncEnumerable<MovieToGetDto> GetAllMovies(CancellationToken cancellationToken)
		{
			List<MovieToGetDto> moviesToReturn;

			lock (movies)
			{
				moviesToReturn = movies.ToList();
			}

			return moviesToReturn.ToAsyncEnumerable();
		}

		public Task<MovieToGetDto> GetMovie(MovieId movieId, CancellationToken cancellationToken)
		{
			lock (movies)
			{
				var movie = FindMovie(movieId);
				return Task.FromResult(movie);
			}
		}

		public Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken)
		{
			lock (movies)
			{
				var movie = FindMovie(movieId);
				movies.Remove(movie);
			}

			return Task.CompletedTask;
		}

		private MovieToGetDto FindMovie(MovieId movieId)
		{
			var movie = movies.FirstOrDefault(m => m.Id == movieId);
			if (movie == null)
			{
				throw new NotFoundException($"The movie with id {movieId} was not found among movies to get");
			}

			return movie;
		}
	}
}
