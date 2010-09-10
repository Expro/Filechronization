/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-01
 * Time: 18:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using Patterns;

namespace CodeExecutionTools.Logging
{
	public interface ILogHandler
	{
		void Write(LogEntry entry);
		ICollection<LogEntry> Read();
		
		event EventHandler<ProgressEventArgs> Progress;
	}
}
