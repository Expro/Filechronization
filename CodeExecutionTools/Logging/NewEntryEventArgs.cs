/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-29
 * Time: 20:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace CodeExecutionTools.Logging
{
	public delegate void NewEntryEventHandler(object sender, NewEntryEventArgs e);
	
	#region Comment
	/// <summary>
	/// Description of NewEntryEventArgs.
	/// </summary>
	#endregion
	public class NewEntryEventArgs: EventArgs
	{
		private LogEntry entry;
		
		public NewEntryEventArgs(LogEntry entry)
		{
			if (entry == null)
				throw new ArgumentNullException("entry");
			
			this.entry = entry;
		}
		
		public LogEntry Entry
		{
			get {return entry;}
			set {entry = value;}
		}
	}
}