﻿using System;
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
	internal class InMemoryMoviesToSeeRepository : IMoviesToSeeRepository
	{
		private readonly List<MovieToSeeDto> movies = new List<MovieToSeeDto>();

		public InMemoryMoviesToSeeRepository()
		{
			movies.Add(new MovieToSeeDto
			{
				Id = new MovieId("11"),
				MovieInfo = new MovieInfo
				{
					Title = "Большой куш",
					Year = 2000,
					MovieUri = new Uri("https://www.kinopoisk.ru/film/526/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_526.jpg"),
					Directors = new[] { "Гай Ричи" },
					Cast = new[] { "Джейсон Стэйтем", "Стивен Грэм", "Брэд Питт", "Алан Форд", "Робби Ги", },
					Rating = new MovieRating(8.536M, 333757),
					Duration = TimeSpan.FromMinutes(104),
					Genres = new[] { "криминал", "комедия", "боевик", },
					Summary = "Четырехпалый Френки должен был переправить краденый алмаз из Англии в США своему боссу Эви. Но вместо этого герой попадает в эпицентр больших неприятностей. Сделав ставку на подпольном боксерском поединке, Френки попадает в круговорот весьма нежелательных событий.\n\n"
							  +
							  "Вокруг героя и его груза разворачивается сложная интрига с участием множества колоритных персонажей лондонского дна — русского гангстера, троих незадачливых грабителей, хитрого боксера и угрюмого громилы грозного мафиози. Каждый норовит в одиночку сорвать Большой Куш.",
				},
			});

			movies.Add(new MovieToSeeDto
			{
				Id = new MovieId("12"),
				MovieInfo = new MovieInfo
				{
					Title = "Марсианин",
					Year = 2015,
					MovieUri = new Uri("https://www.kinopoisk.ru/film/841700/"),
					PosterUri = new Uri("https://st.kp.yandex.net/images/film_iphone/iphone360_841700.jpg"),
					Directors = new[] { "Ридли Скотт" },
					Cast = new[] { "Мэтт Дэймон", "Джессика Честейн", "Чиветель Эджиофор", "Кристен Уиг", "Джефф Дэниелс", },
					Rating = new MovieRating(7.679M, 277474),
					Duration = TimeSpan.FromMinutes(144),
					Genres = new[] { "фантастика", "приключения", "драма", "приключения", },
					Summary = "Марсианская миссия «Арес-3» в процессе работы была вынуждена экстренно покинуть планету из-за надвигающейся песчаной бури. Инженер и биолог Марк Уотни получил повреждение скафандра во время песчаной бури. Сотрудники миссии, посчитав его погибшим, эвакуировались с планеты, оставив Марка одного.\n\n"
							  +
							  "Очнувшись, Уотни обнаруживает, что связь с Землёй отсутствует, но при этом полностью функционирует жилой модуль. Главный герой начинает искать способ продержаться на имеющихся запасах еды, витаминов, воды и воздуха ещё 4 года до прилёта следующей миссии «Арес-4».\n\n",
				},
			});
		}

		public Task CreateMovieToSee(MovieToSeeDto movieToSee, CancellationToken cancellationToken)
		{
			lock (movies)
			{
				movies.Add(movieToSee);
			}

			return Task.CompletedTask;
		}

		public IAsyncEnumerable<MovieToSeeDto> ReadMoviesToSee(CancellationToken cancellationToken)
		{
			List<MovieToSeeDto> moviesToReturn;

			lock (movies)
			{
				moviesToReturn = movies.ToList();
			}

			return moviesToReturn.ToAsyncEnumerable();
		}
	}
}
