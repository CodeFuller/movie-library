﻿using System;
using System.Collections.Generic;
using System.Linq;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Models
{
	public class InputMovieInfoViewModel
	{
		public MovieInfoViewModel SourceMovieInfo { get; }

		public string Title { get; set; }

		public int? Year { get; set; }

		public Uri MovieUri { get; set; }

		public Uri PosterUri { get; set; }

		public IReadOnlyList<string> Directors { get; set; }

		public IReadOnlyList<string> Cast { get; set; }

		public decimal? RatingValue { get; set; }

		public int? RatingVotesNumber { get; set; }

		public TimeSpan? Duration { get; set; }

		public IReadOnlyList<string> Genres { get; set; }

		public string Summary { get; set; }

		public InputMovieInfoViewModel()
		{
		}

		public InputMovieInfoViewModel(MovieInfoModel model)
		{
			SourceMovieInfo = new MovieInfoViewModel(model);

			Title = model.Title;
			Year = model.Year;
			MovieUri = model.MovieUri;
			PosterUri = model.PosterUri;
			Directors = model.Directors?.ToList() ?? new List<string>();
			Cast = model.Cast?.ToList() ?? new List<string>();
			RatingValue = model.Rating?.Value;
			RatingVotesNumber = model.Rating?.VotesNumber;
			Duration = model.Duration;
			Genres = model.Genres?.ToList() ?? new List<string>();
			Summary = model.Summary;
		}

		public MovieInfoModel ToMovieInfo()
		{
			return new MovieInfoModel
			{
				Title = Title,
				Year = Year,
				MovieUri = MovieUri,
				PosterUri = PosterUri,
				Directors = Directors,
				Cast = Cast,
				Rating = RatingValue != null ? new MovieRatingModel(RatingValue.Value, RatingVotesNumber) : null,
				Duration = Duration,
				Genres = Genres,
				Summary = Summary,
			};
		}
	}
}
