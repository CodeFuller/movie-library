using System.Collections.Generic;

namespace MovieLibrary.Internal
{
	public static class Roles
	{
		public const string Administrator = "Administrator";

		public const string CanAddMoviesToGet = "CanAddMoviesToGet";

		public const string CanReadMoviesToGet = "CanReadMoviesToGet";

		public const string CanDeleteMoviesToGet = "CanDeleteMoviesToGet";

		public const string CanAddMoviesToSee = "CanAddMoviesToSee";

		public const string CanReadMoviesToSee = "CanReadMoviesToSee";

		public const string CanMarkMoviesAsSeen = "CanMarkMoviesAsSeen";

		public const string CanDeleteMoviesToSee = "CanDeleteMoviesToSee";

		// Combined roles
		public const string CanAddOrReadMoviesToGet = "CanAddMoviesToGet, CanReadMoviesToGet";
		public const string CanAddOrReadMoviesToSee = "CanAddMoviesToSee, CanReadMoviesToSee";

		public static IEnumerable<string> All
		{
			get
			{
				yield return Administrator;
				yield return CanAddMoviesToGet;
				yield return CanReadMoviesToGet;
				yield return CanDeleteMoviesToGet;
				yield return CanAddMoviesToSee;
				yield return CanReadMoviesToSee;
				yield return CanMarkMoviesAsSeen;
				yield return CanDeleteMoviesToSee;
			}
		}
	}
}
