using System.Runtime.Serialization;

namespace MovieLibrary.Logic.Kinopoisk.DataContracts
{
	[DataContract]
	internal class GetKPFilmDetailViewResponse
	{
		[DataMember(Name = "data")]
		public FilmDetailViewDataContract Data { get; set; }
	}
}
