using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal static class ResponseAssert
	{
		public static async Task VerifyPageLoaded(HttpResponseMessage response, HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
			[CallerFilePath] string callerFilePath = null, [CallerMemberName] string snapshotName = null)
		{
			response.StatusCode.Should().Be(expectedStatusCode);

			var content = await response.Content.ReadAsStringAsync();
			content = UnifyPageContent(content);

			var snapshotFileName = GetSnapshotFileName(callerFilePath, snapshotName);

			if (File.Exists(snapshotFileName))
			{
				var snapshotContent = await File.ReadAllTextAsync(snapshotFileName);
				ComparePageContent(content, snapshotContent);
			}
			else
			{
#if DEBUG
				StoreSnapshot(snapshotFileName, content);
				Assert.Inconclusive("The test was executed in dump-snapshot mode");
#endif

				Assert.Fail($"The snapshot file is missing: {snapshotFileName}");
			}
		}

		private static void StoreSnapshot(string snapshotFileName, string content)
		{
			// We save snapshot not in tests bin directory, but in the sources.
			var saveSnapshotFileName = Path.Combine("..", "..", "..", snapshotFileName);
			var directoryName = Path.GetDirectoryName(saveSnapshotFileName);
			Directory.CreateDirectory(directoryName);

			File.WriteAllText(saveSnapshotFileName, content);
		}

		private static string GetSnapshotFileName(string callerFilePath, string snapshotName)
		{
			var snapshotDirectoryName = Path.GetFileNameWithoutExtension(Path.GetFileName(callerFilePath));

			var snapshotFileName = Path.Combine("snapshots", snapshotDirectoryName, snapshotName);
			snapshotFileName = Path.ChangeExtension(snapshotFileName, "html");

			return snapshotFileName;
		}

		private static string UnifyPageContent(string pageContent)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(pageContent);
			htmlDoc.OptionWriteEmptyNodes = true;

			foreach (var aft in htmlDoc.DocumentNode.SelectNodes("//input[@name='__RequestVerificationToken']") ?? Enumerable.Empty<HtmlNode>())
			{
				aft.SetAttributeValue("value", "Some AFT Value");
			}

			return htmlDoc.DocumentNode.OuterHtml;
		}

		private static void ComparePageContent(string actualContent, string expectedContent)
		{
			var actualContentLines = SplitContentIntoLines(actualContent);
			var expectedContentLines = SplitContentIntoLines(expectedContent);

			actualContentLines.Should().BeEquivalentTo(expectedContentLines, x => x.WithStrictOrdering());
		}

		private static IReadOnlyList<string> SplitContentIntoLines(string content)
		{
			return content.Split(Environment.NewLine);
		}

		public static void VerifyRedirect(HttpResponseMessage response, Uri expectedRedirectUri)
		{
			response.StatusCode.Should().Be(HttpStatusCode.Redirect);
			response.Headers.Location.Should().Be(expectedRedirectUri);
		}

		public static void VerifyMovedPermanently(HttpResponseMessage response, Uri expectedRedirectUri)
		{
			response.StatusCode.Should().Be(HttpStatusCode.MovedPermanently);
			response.Headers.Location.Should().Be(expectedRedirectUri);
		}
	}
}
