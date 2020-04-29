using System.Collections.Generic;

namespace MovieLibrary.Internal
{
	internal static class Roles
	{
		public static string MoviesToGetAdderRole => "MoviesToGetAdder";

		public static string MoviesToGetReaderRole => "MoviesToGetReader";

		public static string MoviesToSeeAdderRole => "MoviesToSeeAdder";

		public static string MoviesToSeeReaderRole => "MoviesToSeeReader";

		public static string CanMarkMoviesAsSeenRole => "CanMarkMoviesAsSeen";

		public static string CanDeleteMoviesToGetRole => "CanDeleteMoviesToGet";

		public static string CanDeleteMoviesToSeeRole => "CanDeleteMoviesToSee";

		public static IEnumerable<string> All
		{
			get
			{
				yield return MoviesToGetAdderRole;
				yield return MoviesToGetReaderRole;
				yield return MoviesToSeeAdderRole;
				yield return MoviesToSeeReaderRole;
				yield return CanMarkMoviesAsSeenRole;
				yield return CanDeleteMoviesToGetRole;
				yield return CanDeleteMoviesToSeeRole;
			}
		}
	}
}
