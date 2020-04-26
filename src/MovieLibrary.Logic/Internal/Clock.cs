using System;
using MovieLibrary.Logic.Interfaces;

namespace MovieLibrary.Logic.Internal
{
	internal class Clock : IClock
	{
		public DateTimeOffset Now => DateTimeOffset.Now;
	}
}
