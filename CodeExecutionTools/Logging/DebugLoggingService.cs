/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-29
 * Time: 19:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;

namespace CodeExecutionTools.Logging
{
	#region Comment
	/// <summary>
	/// Description of DebugLoggingService.
	/// </summary>
	#endregion
	public class DebugLoggingService: AbstractLoggingService
	{	
		public DebugLoggingService(bool isConsoleDefaultOutput = true): base(isConsoleDefaultOutput)
		{
		}
		
		public override void Log(string message, string[] tags = null, object sender = null, EntryCategory category = EntryCategory.Information)
		{
			#if DEBUG
			base.Log(message, tags, sender, category);
			#endif
		}
	}
}
