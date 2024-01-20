using System;

namespace MovieLibrary.Exceptions
{
	[Serializable]
	public class UserManagementException : Exception
	{
		public UserManagementException()
		{
		}

		public UserManagementException(string message)
			: base(message)
		{
		}

		public UserManagementException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
