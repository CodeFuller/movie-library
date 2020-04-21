﻿using System;

namespace MovieLibrary.Logic.Dto
{
	public class MovieId
	{
		private readonly string value;

		public MovieId(string value)
		{
			this.value = value ?? throw new ArgumentNullException(nameof(value));
		}
	}
}
