/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-29
 * Time: 19:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace CodeExecutionTools.Logging
{
	#region Comment
	/// <summary>
	/// Description of ILoggingService.
	/// </summary>
	#endregion
	public interface ILoggingService
	{
		void Log(string message, string[] tags = null, object sender = null, EntryCategory category = EntryCategory.Information);
		void Log(LogEntry entry);
		
		ILoggingService Parent {get; set;}
		
		ICollection<ILogHandler> LogHandlers {get;}
		
		event NewSenderEventHandler NewSender;
		event NewTagEventHandler NewTag;
		event NewEntryEventHandler NewEntry;
	}
}
