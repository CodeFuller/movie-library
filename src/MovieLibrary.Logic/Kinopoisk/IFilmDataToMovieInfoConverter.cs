using MovieLibrary.Logic.Kinopoisk.DataContracts;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal interface IFilmDataToMovieInfoConverter
	{
		MovieInfoModel Convert(FilmDetailViewDataContract data);
	}
}
