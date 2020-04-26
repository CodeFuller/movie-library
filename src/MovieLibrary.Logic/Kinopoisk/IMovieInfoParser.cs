using System;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal interface IMovieInfoParser
	{
		MovieInfoModel ParseMovieInfo(string content, Uri sourceUri);
	}
}
