using System;

namespace MovieLibrary.Logic.Interfaces
{
	internal interface IClock
	{
		DateTimeOffset Now { get; }
	}
}
