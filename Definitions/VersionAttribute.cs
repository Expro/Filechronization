/*
 *
 * User: Expro
 * Date: 2010-07-21
 * Time: 17:09
 * 
 * 
 */
using System;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Version of code, dividen into major, minor and compilation parts.
	/// </summary>
	#endregion
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class VersionAttribute: Attribute
	{
		private int major;
		private int minor;
		private int build;
		
		public VersionAttribute(int major, int minor, int build)
		{
			this.major = major;
			this.minor = minor;
			this.build = build;
		}
		
		public int Major
		{
			get {return major;}
		}
		
		public int Minor
		{
			get {return minor;}
		}
		
		public int Build
		{
			get {return build;}
		}
	}
}
