using System;
using System.Linq;
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

		private readonly IIdGenerator<ObjectId> idGenerator;

		public MoviesToGetRepository(IMongoCollection<MovieToGetDocument> collection, IIdGenerator<ObjectId> idGenerator)
		{
			this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
			this.idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
		}

		public async Task<MovieId> AddMovie(MovieToGetModel movie, CancellationToken cancellationToken)
		{
			var document = movie.ToDocument();
			document.Id = idGenerator.GenerateId();

			var insertOptions = new InsertOneOptions
			{
				BypassDocumentValidation = false,
			};

			await collection.InsertOneAsync(document, insertOptions, cancellationToken);

			return document.Id.ToMovieId();
		}

		public IQueryable<MovieToGetModel> GetAllMovies()
		{
			// TODO: Remove collection instantiation (ToList()) and return true IQueryable.
			// This will be possible after Azure Cosmos DB supports $toString operator available since version 4.0 of MongoDB.
			return collection
				.AsQueryable()
				.ToList()
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
						Countries = d.MovieInfo.Countries,
						SummaryParagraphs = d.MovieInfo.GetSummaryParagraphs(),
					},
					TimestampOfAddingToGetList = d.TimestampOfAddingToGetList,
				})
				.AsQueryable();
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
				throw new NotFoundException($"The movie with id {movieId} was not found among movies to get (DeletedCount = {deleteResult.DeletedCount})");
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
