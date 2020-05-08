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
	internal class MoviesToSeeRepository : IMoviesToSeeRepository
	{
		private readonly IMongoCollection<MovieToSeeDocument> collection;

		public MoviesToSeeRepository(IMongoCollection<MovieToSeeDocument> collection)
		{
			this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
		}

		public async Task<MovieId> AddMovie(MovieToSeeModel movie, CancellationToken cancellationToken)
		{
			var document = movie.ToDocument();

			var insertOptions = new InsertOneOptions
			{
				BypassDocumentValidation = false,
			};

			await collection.InsertOneAsync(document, insertOptions, cancellationToken);

			return document.Id.ToMovieId();
		}

		public IQueryable<MovieToSeeModel> GetAllMovies()
		{
			return collection
				.AsQueryable()
				.Select(d => new MovieToSeeModel
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
					TimestampOfAddingToSeeList = d.TimestampOfAddingToSeeList,
				});
		}

		public async Task<MovieToSeeModel> GetMovie(MovieId movieId, CancellationToken cancellationToken)
		{
			var filter = BuildMovieFilter(movieId);

			var options = new FindOptions<MovieToSeeDocument>
			{
				Limit = 1,
			};

			var cursor = await collection.FindAsync(filter, options, cancellationToken);
			var document = await cursor.FirstOrDefaultAsync(cancellationToken);

			if (document == null)
			{
				throw new NotFoundException($"The movie with id {movieId} was not found among movies to see");
			}

			return document.ToModel();
		}

		public async Task DeleteMovie(MovieId movieId, CancellationToken cancellationToken)
		{
			var filter = BuildMovieFilter(movieId);
			var deleteResult = await collection.DeleteOneAsync(filter, cancellationToken);

			if (deleteResult.DeletedCount != 1)
			{
				throw new NotFoundException($"The movie with id {movieId} was not found among movies to see");
			}
		}

		private static FilterDefinition<MovieToSeeDocument> BuildMovieFilter(MovieId movieId)
		{
			var objectId = new ObjectId(movieId.Value);

			return new FilterDefinitionBuilder<MovieToSeeDocument>()
				.Where(d => d.Id == objectId);
		}
	}
}
