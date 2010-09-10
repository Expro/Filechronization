/*
 *
 * User: Expro
 * Date: 2010-07-11
 * Time: 22:28
 * 
 * 
 */
using System;
using System.Runtime.Serialization;

namespace CodeManagement
{
	#region Comment
	/// <summary>
	/// Desctiption of MissingComponentException.
	/// </summary>
	#endregion
	[Serializable]
	public class MissingSharedCodeException : Exception, ISerializable
	{
		public MissingSharedCodeException()
		{
		}

	 	public MissingSharedCodeException(string message): base(message)
		{
		}

		public MissingSharedCodeException(string message, Exception innerException): base(message, innerException)
		{
		}

		// This constructor is needed for serialization.
		protected MissingSharedCodeException(SerializationInfo info, StreamingContext context): base(info, context)
		{
		}
	}
}