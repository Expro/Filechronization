/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-30
 * Time: 02:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace CodeExecutionTools.Logging
{
	#region Comment
	/// <summary>
	/// Description of LoggingService.
	/// </summary>
	#endregion
	public static class LoggingService
	{
		private static ILoggingService debug;
		private static ILoggingService trace;
		
		static LoggingService()
		{
			debug = new DebugLoggingService();
			trace = new TraceLoggingService();
		}
		
		public static ILoggingService Trace
		{
			get {return trace;}
		}
		
		public static ILoggingService Debug
		{
			get {return debug;}
		}
	}
}
