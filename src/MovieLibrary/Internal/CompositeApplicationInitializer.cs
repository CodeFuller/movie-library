using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieLibrary.Internal
{
	internal class CompositeApplicationInitializer : ICompositeApplicationInitializer
	{
		private readonly IEnumerable<IApplicationInitializer> initializers;

		public CompositeApplicationInitializer(IEnumerable<IApplicationInitializer> initializers)
		{
			this.initializers = initializers ?? throw new ArgumentNullException(nameof(initializers));
		}

		public async Task Initialize(CancellationToken cancellationToken)
		{
			foreach (var initializer in initializers)
			{
				await initializer.Initialize(cancellationToken);
			}
		}
	}
}
