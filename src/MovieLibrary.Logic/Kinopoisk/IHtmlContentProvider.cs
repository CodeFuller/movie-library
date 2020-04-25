using System;
using System.Threading;
using System.Threading.Tasks;

namespace MovieLibrary.Logic.Kinopoisk
{
	internal interface IHtmlContentProvider
	{
		Task<string> GetHtmlPageContent(Uri pageUri, CancellationToken cancellationToken);
	}
}
