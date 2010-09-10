/*
 *
 * User: Expro
 * Date: 2010-07-17
 * Time: 22:44
 * 
 * 
 */
using System;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Marks class as code necessery for proper work of application.
	/// </summary>
	#endregion
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class ModuleAttribute: Attribute
	{
		private string condition;
		
		public ModuleAttribute()
		{
			this.condition = "";
		}
		
		public ModuleAttribute(string condition)
		{
			this.condition = condition.ToUpper();
		}
		
		public string Condition
		{
			get {return condition;}
		}
	}
}
