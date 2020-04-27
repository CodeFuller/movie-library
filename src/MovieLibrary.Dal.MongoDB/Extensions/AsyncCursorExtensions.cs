using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using MongoDB.Driver;

namespace MovieLibrary.Dal.MongoDB.Extensions
{
	internal static class AsyncCursorExtensions
	{
		public static async IAsyncEnumerable<TDocument> AsAsyncEnumerable<TDocument>(this IAsyncCursor<TDocument> cursor, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			while (await cursor.MoveNextAsync(cancellationToken))
			{
				foreach (var document in cursor.Current)
				{
					yield return document;
				}
			}
		}
	}
}
