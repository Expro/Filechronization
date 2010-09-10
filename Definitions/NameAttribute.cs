/*
 *
 * User: Expro
 * Date: 2010-07-11
 * Time: 23:21
 * 
 * 
 */
using System;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Name of code segment.
	/// </summary>
	#endregion
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class NameAttribute: Attribute
	{
		private string value;
		
		public NameAttribute(string value)
		{
			this.value = value;
		}
		
		public string Value
		{
			get {return value;}
		}
	}
}
