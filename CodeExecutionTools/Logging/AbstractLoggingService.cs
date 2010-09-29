/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-29
 * Time: 20:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CodeExecutionTools.Logging
{
	#region Comment
	/// <summary>
	/// 	Provides basic implemetation of <see cref="ILoggingService"></see>.
	/// </summary>
	/// <remarks>
	/// 	This implementation provides <see cref="LogEntry"></see> exchange over application domains.
	/// </remarks>
	#endregion
	public abstract class AbstractLoggingService: MarshalByRefObject, ILoggingService
	{
		#region Fields
		private ISet<LogEntry> entries;
		private ISet<object> senders;
		private ISet<string> tags;
		private ICollection<ILogHandler> logHandlers;
		
		private ILoggingService parent;
		#endregion
		
		#region Protected Methods
		protected virtual void OnNewEntry(LogEntry entry)
		{
			if (NewEntry != null)
				NewEntry(this, new NewEntryEventArgs(entry));
		}
		
		protected virtual void OnNewTag(string tag)
		{
			if (NewTag != null)
				NewTag(this, new NewTagEventArgs(tag));
		}
		
		protected virtual void OnNewSender(object sender)
		{
			if (NewSender != null)
				NewSender(this, new NewSenderEventArgs(sender));
		}
		#endregion
		
		#region Protected Properties
		protected ISet<object> Senders
		{
			get {return senders;}
		}
		
		protected ISet<string> Tags
		{
			get {return tags;}
		}
		
		protected ISet<LogEntry> Entries
		{
			get {return entries;}
		}
		#endregion
		
		#region Constructors
		public AbstractLoggingService(bool isConsoleDefaultOutput = true)
		{
			this.entries = new HashSet<LogEntry>();
			this.senders = new HashSet<object>();
			this.tags = new HashSet<string>();
			
			this.logHandlers = new HashSet<ILogHandler>();
			
			if (isConsoleDefaultOutput)
				LogHandlers.Add(new ConsoleLogHandler());
		}
		#endregion
		
		#region Public Methods
		public virtual void Log(LogEntry entry)
		{
			ISet<ILogHandler> handlersToRemove = new HashSet<ILogHandler>();
			
			lock (this)
			{
				if (Parent != null)
					Parent.Log(entry);
				else
				{	
					if (!Senders.Contains(entry.Sender))
					{
						Senders.Add(entry.Sender);
						OnNewSender(entry.Sender);
					}
					
					if (entry.Tags != null)
					{
						foreach (string tag in entry.Tags)
						{
							if (!Tags.Contains(tag))
							{
								Tags.Add(tag);
								OnNewTag(tag);
							}
						}
					}
					
					Entries.Add(entry);
					OnNewEntry(entry);
					
					foreach (ILogHandler handler in LogHandlers)
					{
						try
						{
							handler.Write(entry);
						}
						catch (ObjectDisposedException)
						{
							handlersToRemove.Add(handler);
						}
					}
					
					foreach (ILogHandler handler in handlersToRemove)
						LogHandlers.Remove(handler);
				}
			}
		}
		
		public virtual void Log(string message, string[] tags = null, object sender = null, EntryCategory category = EntryCategory.Information)
		{
			LogEntry entry = new LogEntry(DateTime.Now, message, tags, sender, category);
				
			Log(entry);
		}
		
		public void Information(string message, string[] tags = null, object sender = null)
		{
			Log(message, tags, sender, EntryCategory.Information);
		}
		
		public void Warning(string message, string[] tags = null, object sender = null)
		{
			Log(message, tags, sender, EntryCategory.Warning);
		}
		
		public void Error(string message, string[] tags = null, object sender = null)
		{
			Log(message, tags, sender, EntryCategory.Error);
		}
		#endregion
		
		#region Public Properties
		public ILoggingService Parent
		{
			get
			{
				ILoggingService result;
				
				lock (this)
				{
					result = parent;
				}
				
				return result;
			}
			
			set
			{
				lock (this)
				{
					parent = value;
				}
			}
		}
		
		public ICollection<ILogHandler> LogHandlers
		{
			get
			{
				ICollection<ILogHandler> result;
				
				lock (this)
				{
					result = logHandlers;
				}
				
				return result;
			}
		}
		#endregion
		
		#region Events
		public event NewSenderEventHandler NewSender;
		public event NewTagEventHandler NewTag;
		public event NewEntryEventHandler NewEntry;
		#endregion
	}
}
