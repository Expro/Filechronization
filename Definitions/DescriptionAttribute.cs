/*
 *
 * User: Expro
 * Date: 2010-07-12
 * Time: 00:17
 * 
 * 
 */

using System;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Information about code and services provided by it.
	/// </summary>
	/// <remarks>
	/// 	May be used multiple times, each attribute is one line of description.
	/// </remarks>
	#endregion
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public sealed class DescriptionAttribute: Attribute
	{
		private string text;
		
		public DescriptionAttribute(string text)
		{
			this.text = text;
		}
		
		public string Text
		{
			get {return text;}
		}
	}
}