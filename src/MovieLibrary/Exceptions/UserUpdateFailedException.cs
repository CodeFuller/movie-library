using System;
using System.Runtime.Serialization;

namespace MovieLibrary.Exceptions
{
	[Serializable]
	public class UserUpdateFailedException : Exception
	{
		public UserUpdateFailedException()
		{
		}

		public UserUpdateFailedException(string message)
			: base(message)
		{
		}

		public UserUpdateFailedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UserUpdateFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}
