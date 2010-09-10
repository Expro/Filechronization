/*
 *
 * User: Expro
 * Date: 2010-07-12
 * Time: 00:23
 * 
 * 
 */

using System;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Describes who created code.
	/// </summary>
	/// <remarks>
	/// 	May be used multiple times to provide informations about many authors.
	/// </remarks>
	#endregion
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public sealed class AuthorAttribute: Attribute
	{
		private string name;
		private string email;
		
		public AuthorAttribute(string name, string email)
		{
			this.name = name;
			this.email = email;
		}
		
		public string Email
		{
			get {return email;}
		}
		
		public string Name
		{
			get {return name;}
		}
	}
}