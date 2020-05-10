using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MovieLibrary.IntegrationTests.Internal
{
	internal static class ResponseAssert
	{
		public static async Task VerifyPageLoaded(HttpResponseMessage response, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string snapshotName = null)
		{
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

			var content = await response.Content.ReadAsStringAsync();
			content = UnifyPageContent(content);

			var snapshotFileName = GetSnapshotFileName(callerFilePath, snapshotName);

			if (File.Exists(snapshotFileName))
			{
				var snapshotContent = File.ReadAllText(snapshotFileName);
				ComparePageContent(content, snapshotContent);
			}
			else
			{
				StoreSnapshot(snapshotFileName, content);

				Assert.Inconclusive("The test was executed in dump-snapshot mode");
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

			CompareContentLines(actualContentLines, expectedContentLines);
		}

		private static void CompareContentLines(IReadOnlyList<string> actualContent, IReadOnlyList<string> expectedContent)
		{
			for (var i = 0; i < Math.Min(actualContent.Count, expectedContent.Count); ++i)
			{
				if (!String.Equals(actualContent[i], expectedContent[i], StringComparison.Ordinal))
				{
					var messageBuilder = new StringBuilder();
					messageBuilder.AppendLine($"Content differs at line {i + 1}:");
					messageBuilder.AppendLine();
					messageBuilder.AppendLine($"Expected: '{expectedContent[i]}'");
					messageBuilder.AppendLine($"Actual:   '{actualContent[i]}'");

					Assert.Fail(messageBuilder.ToString());
				}
			}

			Assert.AreEqual(expectedContent.Count, actualContent.Count, "Content lengths differ");
		}

		private static IReadOnlyList<string> SplitContentIntoLines(string content)
		{
			return content.Split(Environment.NewLine);
		}

		public static void VerifyRedirect(HttpResponseMessage response, Uri expectedRedirectUri)
		{
			Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
			Assert.AreEqual(expectedRedirectUri, response.Headers.Location);
		}
	}
}
