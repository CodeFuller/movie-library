using System;

namespace MovieLibrary.Logic.Models
{
	public class MovieId
	{
		private static StringComparison IdComparison => StringComparison.Ordinal;

		public string Value { get; }

		public MovieId(string value)
		{
			Value = value ?? throw new ArgumentNullException(nameof(value));
		}

		public static bool operator ==(MovieId v1, MovieId v2)
		{
			if (Object.ReferenceEquals(v1, null) || Object.ReferenceEquals(v2, null))
			{
				return Object.ReferenceEquals(v1, null) && Object.ReferenceEquals(v2, null);
			}

			return v1.Equals(v2);
		}

		public static bool operator !=(MovieId v1, MovieId v2)
		{
			return !(v1 == v2);
		}

		public override bool Equals(object obj)
		{
			return obj is MovieId cmp && Equals(cmp);
		}

		protected bool Equals(MovieId other)
		{
			return String.Equals(Value, other.Value, IdComparison);
		}

		public override int GetHashCode()
		{
			return Value?.GetHashCode(IdComparison) ?? 0;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
