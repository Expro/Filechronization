/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-01
 * Time: 18:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Patterns;

//TODO: dodac obsluge zdarzenia Progress
namespace CodeExecutionTools.Logging
{
	public class ConsoleLogHandler: ILogHandler
	{
		protected virtual void OnProgress(ProgressEventArgs e)
		{
			if (Progress != null)
				Progress(this, e);
		}
		
		public ConsoleLogHandler()
		{
		}
		
		public void Write(LogEntry entry)
		{
			switch (entry.Category)
			{
				case EntryCategory.Information:
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Out.WriteLine(entry.ToString());
					break;
				case EntryCategory.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Out.WriteLine(entry.ToString());
					Console.Error.WriteLine(entry.ToString());
					break;
				case EntryCategory.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Error.WriteLine(entry.ToString());
				break;
				default:
				break;
			}
			
			Console.WriteLine(Environment.NewLine);
		}
		
		public System.Collections.Generic.ICollection<LogEntry> Read()
		{
			throw new NotSupportedException("Reading from console is currently not supported.");
		}
		
		public event EventHandler<ProgressEventArgs> Progress;
	}
}
