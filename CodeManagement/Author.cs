/*
 *
 * User: Expro
 * Date: 2010-07-29
 * Time: 12:46
 * 
 * 
 */
using System;
using System.Diagnostics.Contracts;

namespace CodeManagement
{
	[Serializable]
	public class Author
	{
		private string name;
		private string email;
		
		public Author(string name, string email)
		{
			Contract.Requires(name != null);
			Contract.Requires(email != null);
			
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
