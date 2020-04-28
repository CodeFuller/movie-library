﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Dal.MongoDB.Extensions;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Dal.MongoDB.Repositories
{
	internal class MoviesToSeeRepository : IMoviesToSeeRepository
	{
		private readonly IMongoCollection<MovieToSeeDocument> collection;

		public MoviesToSeeRepository(IMongoCollection<MovieToSeeDocument> collection)
		{
			this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
		}

		public async Task AddMovie(MovieToSeeModel movie, CancellationToken cancellationToken)
		{
			var document = movie.ToDocument();

			var insertOptions = new InsertOneOptions
			{
				BypassDocumentValidation = false,
			};

			await collection.InsertOneAsync(document, insertOptions, cancellationToken);
		}

		public async IAsyncEnumerable<MovieToSeeModel> GetAllMovies([EnumeratorCancellation] CancellationToken cancellationToken)
		{
			using var cursor = await collection.FindAsync(FilterDefinition<MovieToSeeDocument>.Empty, cancellationToken: cancellationToken);

			await foreach (var document in cursor.AsAsyncEnumerable(cancellationToken))
			{
				yield return document.ToModel();
			}
		}
	}
}