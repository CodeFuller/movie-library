using System.Threading;
using System.Threading.Tasks;

namespace MovieLibrary.Internal
{
	public interface IApplicationInitializer
	{
		Task Initialize(CancellationToken cancellationToken);
	}
}
