using System;
using System.Runtime.Serialization;

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

		protected UserManagementException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}
