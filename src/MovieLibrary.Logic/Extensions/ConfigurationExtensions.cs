using System;
using Microsoft.Extensions.Configuration;

namespace MovieLibrary.Logic.Extensions
{
	public static class ConfigurationExtensions
	{
		public static string GetMovieLibraryConnectionString(this IConfiguration configuration)
		{
			var connectionStringName = configuration["connectionStringName"];
			if (String.IsNullOrEmpty(connectionStringName))
			{
				throw new InvalidOperationException("Connection string name is not set. Define it via setting 'connectionStringName'");
			}

			var connectionString = configuration.GetConnectionString(connectionStringName);
			if (String.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException($"Connection string '{connectionStringName}' is not set. Define it via setting 'connectionStrings:{connectionStringName}'");
			}

			return connectionString;
		}
	}
}
