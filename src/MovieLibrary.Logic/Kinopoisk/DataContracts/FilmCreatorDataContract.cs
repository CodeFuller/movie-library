using System.Runtime.Serialization;

namespace MovieLibrary.Logic.Kinopoisk.DataContracts
{
	[DataContract]
	internal class FilmCreatorDataContract
	{
		[DataMember(Name = "nameRU")]
		public string NameInRussian { get; set; }

		[DataMember(Name = "professionKey")]
		public string ProfessionKey { get; set; }
	}
}
