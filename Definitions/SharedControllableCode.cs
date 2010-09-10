/*
 *
 * User: Expro
 * Date: 2010-07-21
 * Time: 16:05
 * 
 * 
 */
using System;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Shortcut for creation of controllable shared codes.
	/// </summary>
	#endregion
	public abstract class SharedControllableCode: SharedCode, IControllableCode
	{
		public abstract void Start();
		
		public abstract void Pause();
		
		public abstract void Restore();
		
		public abstract void End();
	}
}
