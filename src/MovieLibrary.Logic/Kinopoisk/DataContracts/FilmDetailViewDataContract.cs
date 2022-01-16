using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MovieLibrary.Logic.Kinopoisk.DataContracts
{
	[DataContract]
	internal class FilmDetailViewDataContract
	{
		[DataMember(Name = "ratingData")]
		public FilmRatingDataContract RatingData { get; set; }

		[DataMember(Name = "webUrl")]
		public string WebUrl { get; set; }

		[DataMember(Name = "nameRU")]
		public string NameInRussian { get; set; }

		[DataMember(Name = "year")]
		public int? Year { get; set; }

		[DataMember(Name = "filmLength")]
		public string FilmLength { get; set; }

		[DataMember(Name = "genre")]
		public string Genre { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "creators")]
		public IReadOnlyCollection<IReadOnlyCollection<FilmCreatorDataContract>> Creators { get; set; }
	}
}
