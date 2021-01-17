using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieLibrary.IntegrationTests.Extensions
{
	public static class ListExtensions
	{
		public static void InsertBeforeType<T>(this IList<T> list, T item, Type searchedType)
		{
			var lastIndexForType = list
				.Select((x, i) => (Item: x, Index: i))
				.Where(x => x.Item.GetType().IsAssignableFrom(searchedType))
				.Select(x => x.Index)
				.Cast<int?>()
				.LastOrDefault();

			list.Insert(lastIndexForType ?? list.Count, item);
		}
	}
}
