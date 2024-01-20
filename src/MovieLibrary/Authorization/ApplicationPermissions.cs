using System;
using System.Collections.Generic;

namespace MovieLibrary.Authorization
{
	public static class ApplicationPermissions
	{
#pragma warning disable CA1034 // Nested types should not be visible
		public static class MoviesToGet
#pragma warning restore CA1034 // Nested types should not be visible
		{
			public const string Add = "Permissions.MoviesToGet.Add";
			public const string Read = "Permissions.MoviesToGet.Read";
			public const string MoveToMoviesToSee = "Permissions.MoviesToGet.MoveToMoviesToSee";
			public const string Delete = "Permissions.MoviesToGet.Delete";

			public const string AddOrRead = "Permissions.MoviesToGet.Add, Permissions.MoviesToGet.Read";
		}

#pragma warning disable CA1034 // Nested types should not be visible
		public static class MoviesToSee
#pragma warning restore CA1034 // Nested types should not be visible
		{
			public const string Add = "Permissions.MoviesToSee.Add";
			public const string Read = "Permissions.MoviesToSee.Read";
			public const string MarkAsSeen = "Permissions.MoviesToSee.MarkAsSeen";
			public const string Delete = "Permissions.MoviesToSee.Delete";

			public const string AddOrRead = "Permissions.MoviesToSee.Add, Permissions.MoviesToSee.Read";
		}

		public static IEnumerable<string> All
		{
			get
			{
				yield return MoviesToGet.Add;
				yield return MoviesToGet.Read;
				yield return MoviesToGet.MoveToMoviesToSee;
				yield return MoviesToGet.Delete;

				yield return MoviesToSee.Add;
				yield return MoviesToSee.Read;
				yield return MoviesToSee.MarkAsSeen;
				yield return MoviesToSee.Delete;
			}
		}

		public static bool TryParseApplicationPermissions(this string policyName, out IEnumerable<string> permissions)
		{
			if (String.Equals(policyName, MoviesToGet.AddOrRead, StringComparison.Ordinal))
			{
				permissions = new[] { MoviesToGet.Add, MoviesToGet.Read, };
				return true;
			}

			if (String.Equals(policyName, MoviesToSee.AddOrRead, StringComparison.Ordinal))
			{
				permissions = new[] { MoviesToSee.Add, MoviesToSee.Read, };
				return true;
			}

			if (policyName.StartsWith("Permissions.MoviesToGet", StringComparison.Ordinal) ||
			    policyName.StartsWith("Permissions.MoviesToSee", StringComparison.Ordinal))
			{
				permissions = new[] { policyName, };
				return true;
			}

			permissions = null;
			return false;
		}
	}
}
