using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MovieLibrary.Logic.Kinopoisk.DataContracts
{
	[DataContract]
	internal class FilmCreatorDataContract
	{
		[DataMember(Name = "nameRU")]
		public string NameInRussian { get; set; }

		[DataMember(Name = "nameEn")]
		public string NameInEnglish { get; set; }

		[DataMember(Name = "professionKey")]
		public string ProfessionKey { get; set; }

		public string Name => AllNames.FirstOrDefault(x => !String.IsNullOrWhiteSpace(x));

		private IEnumerable<string> AllNames
		{
			get
			{
				yield return NameInRussian;
				yield return NameInEnglish;
			}
		}
	}
}
