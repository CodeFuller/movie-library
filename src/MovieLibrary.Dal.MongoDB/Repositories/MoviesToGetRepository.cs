using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Dal.MongoDB.Extensions;
using MovieLibrary.Logic.Exceptions;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Dal.MongoDB.Repositories
{
	internal class MoviesToGetRepository : IMoviesToGetRepository
	{
		private readonly IMongoCollection<MovieToGetDocument> collection;

		public MoviesToGetRepository(IMongoCollection<MovieToGetDocument> collection)
		{
			this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
		}

		public async Task AddMovie(MovieToGetModel movie, CancellationToken cancellationToken)
		{
			var document = movie.ToDocument();

			var insertOptions = new InsertOneOptions
			{
				BypassDocumentValidation = false,
			};

			await collection.InsertOneAsync(document, insertOptions, cancellationToken);
		}

		public async IAsyncEnumerable<MovieToGetModel> GetAllMovies([EnumeratorCancellation] CancellationToken cancellationToken)
		{
			using var cursor = await collection.FindAsync(FilterDefinition<MovieToGetDocument>.Empty, cancellationToken: cancellationToken);

			await foreach (var document in cursor.AsAsyncEnumerable(cancellationToken))
			{
				yield return document.ToModel();
			}
		}

		public async Task<MovieToGetModel> GetMovie(MovieId movieId, CancellationToken cancellationToken)
		{
			var filter = BuildMovieFilter(movieId);

			var options = new FindOptions<MovieToGetDocument>
			{
				Limit = 1,
			};

			var cursor = await collection.FindAsync(filter, options, cancellationToken);
			var document = await cursor.FirstOrDefaultAsync(cancellationToken);

			if (document == null)
			{
				throw new NotFoundException($"The movie with id {movieId} was not found among movies to get");
			}

			return document.ToModel();
		}

		public async Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken)
		{
			var filter = BuildMovieFilter(movieId);
			var deleteResult = await collection.DeleteOneAsync(filter, cancellationToken);

			if (deleteResult.DeletedCount != 1)
			{
				throw new NotFoundException($"The movie with id {movieId} was not found among movies to get");
			}
		}

		private static FilterDefinition<MovieToGetDocument> BuildMovieFilter(MovieId movieId)
		{
			var objectId = new ObjectId(movieId.Value);

			return new FilterDefinitionBuilder<MovieToGetDocument>()
				.Where(d => d.Id == objectId);
		}
	}
}
