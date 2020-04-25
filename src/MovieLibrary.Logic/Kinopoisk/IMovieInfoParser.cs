using System;
using MovieLibrary.Logic.MoviesInfo;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal interface IMovieInfoParser
	{
		MovieInfo ParseMovieInfo(string content, Uri sourceUri);
	}
}
