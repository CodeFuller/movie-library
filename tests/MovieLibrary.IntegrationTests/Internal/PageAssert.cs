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
	internal static class PageAssert
	{
		public static async Task VerifyResponse(HttpResponseMessage response, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMethodName = null)
		{
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

			var content = await response.Content.ReadAsStringAsync();
			content = UnifyPageContent(content);

			var snapshotFileName = GetSnapshotFileName(callerFilePath, callerMethodName);

			if (File.Exists(snapshotFileName))
			{
				var snapshotContent = File.ReadAllText(snapshotFileName);
				content.Should().Be(snapshotContent);
			}
			else
			{
				// We save snapshot not in tests bin directory, but in the sources.
				var saveSnapshotFileName = Path.Combine("..", "..", "..", snapshotFileName);
				var directoryName = Path.GetDirectoryName(saveSnapshotFileName);
				Directory.CreateDirectory(directoryName);
				File.WriteAllText(saveSnapshotFileName, content);

				Assert.Inconclusive("The test was executed in dump-snapshot mode");
			}
		}

		private static string GetSnapshotFileName(string callerFilePath, string callerMethodName)
		{
			var snapshotDirectoryName = Path.GetFileNameWithoutExtension(Path.GetFileName(callerFilePath));

			var snapshotFileName = Path.Combine("snapshots", snapshotDirectoryName, callerMethodName);
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
	}
}
