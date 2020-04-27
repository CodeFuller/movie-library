using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieLibrary.Logic.Models;

namespace MovieLibrary.Logic.IntegrationTests
{
	internal static class MovieAssert
	{
		public static void AreEqual(MovieToGetModel expected, MovieToGetModel actual)
		{
			DoSanityCheckForComparedObjects(expected, actual, 3);

			Assert.IsNotNull(actual.Id);
			Assert.AreEqual(expected.TimestampOfAddingToGetList, actual.TimestampOfAddingToGetList);
			MovieAssert.AreEqual(expected.MovieInfo, actual.MovieInfo);
		}

		public static void AreEqual(MovieToSeeModel expected, MovieToSeeModel actual)
		{
			DoSanityCheckForComparedObjects(expected, actual, 3);

			Assert.IsNotNull(actual.Id);
			Assert.AreEqual(expected.TimestampOfAddingToSeeList, actual.TimestampOfAddingToSeeList);
			MovieAssert.AreEqual(expected.MovieInfo, actual.MovieInfo);
		}

		public static void AreEqual(MovieInfoModel expected, MovieInfoModel actual)
		{
			DoSanityCheckForComparedObjects(expected, actual, 10);

			Assert.AreEqual(expected.Title, actual.Title);
			Assert.AreEqual(expected.Year, actual.Year);
			Assert.AreEqual(expected.MovieUri, actual.MovieUri);
			Assert.AreEqual(expected.PosterUri, actual.PosterUri);
			CollectionAssert.AreEqual(expected.Directors?.ToList(), actual.Directors?.ToList());
			CollectionAssert.AreEqual(expected.Cast?.ToList(), actual.Cast?.ToList());
			Assert.AreEqual(expected.Rating?.Value, actual.Rating?.Value);
			Assert.AreEqual(expected.Rating?.VotesNumber, actual.Rating?.VotesNumber);
			Assert.AreEqual(expected.Duration, actual.Duration);
			CollectionAssert.AreEqual(expected.Genres?.ToList(), actual.Genres?.ToList());
			Assert.AreEqual(expected.Summary, actual.Summary);
		}

		private static void DoSanityCheckForComparedObjects<TObject>(TObject expected, TObject actual, int propertiesNumber)
		{
			Assert.AreNotSame(expected, actual);

			// Sanity check that all properties are covered.
			// If this check starts failing, update the list of compared properties and increase the counter.
			var properties = expected.GetType().GetProperties();
			Assert.AreEqual(propertiesNumber, properties.Length);
		}
	}
}
