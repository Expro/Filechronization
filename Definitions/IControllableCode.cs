/*
 *
 * User: Expro
 * Date: 2010-07-21
 * Time: 13:59
 * 
 * 
 */
using System;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Code that requires execution, provides methods to control this execution.
	/// </summary>
	/// <remarks>
	/// 	Separated thread for controllable code is provides during code construction.
	/// </remarks>
	#endregion
	public interface IControllableCode: ICode
	{
		#region Comment
		/// <summary>
		/// 	Starts execution of code.
		/// </summary>
		#endregion
		void Start();
		
		#region Comment
		/// <summary>
		/// 	Pauses working code;
		/// </summary>
		#endregion
		void Pause();
		
		#region Comment
		/// <summary>
		/// 	Resumes paused code.
		/// </summary>
		#endregion
		void Restore();
		
		#region Comment
		/// <summary>
		/// 	Stops working code.
		/// </summary>
		#endregion
		void End();
	}
}
