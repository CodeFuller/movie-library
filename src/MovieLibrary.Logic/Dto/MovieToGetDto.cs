﻿using MovieLibrary.Logic.Models;
using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Dto
{
	public class MovieToGetDto
	{
		public MovieId Id { get; set; }

		public MovieInfo MovieInfo { get; set; }
	}
}