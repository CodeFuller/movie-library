﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieLibrary.Dal.MongoDB.Documents;
using MovieLibrary.Dal.MongoDB.Extensions;
using MovieLibrary.Dal.MongoDB.Internal;
using MovieLibrary.Logic.Exceptions;
using MovieLibrary.Logic.Interfaces;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Dal.MongoDB.Repositories
{
	internal class MoviesToGetRepository : IMoviesToGetRepository
	{
		private readonly IMongoCollection<MovieToGetDocument> collection;

		private readonly IDocumentIdGenerator documentIdGenerator;

		public MoviesToGetRepository(IMongoCollection<MovieToGetDocument> collection, IDocumentIdGenerator documentIdGenerator)
		{
			this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
			this.documentIdGenerator = documentIdGenerator ?? throw new ArgumentNullException(nameof(documentIdGenerator));
		}

		public async Task<MovieId> AddMovie(MovieToGetModel movie, CancellationToken cancellationToken)
		{
			var document = movie.ToDocument();
			document.Id = documentIdGenerator.GenerateIdForNewDocument();

			var insertOptions = new InsertOneOptions
			{
				BypassDocumentValidation = false,
			};

			await collection.InsertOneAsync(document, insertOptions, cancellationToken);

			return document.Id.ToMovieId();
		}

		public IQueryable<MovieToGetModel> GetAllMovies()
		{
			return collection
				.AsQueryable()
				.Select(d => new MovieToGetModel
				{
					Id = new MovieId(d.Id.ToString()),
					MovieInfo = new MovieInfoModel
					{
						Title = d.MovieInfo.Title,
						Year = d.MovieInfo.Year,
						MovieUri = d.MovieInfo.MovieUri,
						PosterUri = d.MovieInfo.PosterUri,
						Directors = d.MovieInfo.Directors,
						Cast = d.MovieInfo.Cast,
						Rating = d.MovieInfo.RatingValue != null ? new MovieRatingModel(d.MovieInfo.RatingValue.Value, d.MovieInfo.RatingVotesNumber) : null,
						Duration = d.MovieInfo.Duration,
						Genres = d.MovieInfo.Genres,
						Summary = d.MovieInfo.Summary,
					},
					TimestampOfAddingToGetList = d.TimestampOfAddingToGetList,
				});
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
