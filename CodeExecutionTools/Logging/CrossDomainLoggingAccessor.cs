/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-01
 * Time: 17:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace CodeExecutionTools.Logging
{
	public class CrossDomainLoggingAccessor: MarshalByRefObject
	{
		public CrossDomainLoggingAccessor()
		{
		}
		
		public ILoggingService LocalDebugLoggingService
		{
			get {return LoggingService.Debug;}
		}
		
		public ILoggingService LocalTraceLoggingService
		{
			get {return LoggingService.Trace;}
		}
	}
}
