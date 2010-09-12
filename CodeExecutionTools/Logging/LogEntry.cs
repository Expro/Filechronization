/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-29
 * Time: 19:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace CodeExecutionTools.Logging
{
	#region Comment
	/// <summary>
	/// 	Description of LogEntry.
	/// </summary>
	#endregion
	[Serializable]
	[DataContract(Name = "Entry", Namespace = "Logging")]
	public class LogEntry
	{
		[DataMember(Name = "Tags")]
		private string[] tags;
		[DataMember(Name = "Message")]
		private string message;
		[DataMember(Name = "Sender")]
		private string sender;
		[DataMember(Name = "Category")]
		private EntryCategory category;
		[DataMember(Name = "Time")]
		private DateTime time;
		
		public LogEntry(DateTime time, string message, string[] tags = null, object sender = null, EntryCategory category = EntryCategory.Information)
		{
			Contract.Requires(message != null);
			Contract.Requires(time != null);
			
			this.tags = tags;
			
			if (tags != null)
			{
				for (int i = 0; i < this.tags.Length; i++)	
					this.tags[i] = this.tags[i].ToUpper();
			}
			
			this.message = message;
			
			if (sender != null)
				this.sender = sender.ToString() + " (" + sender.GetHashCode().ToString() + ")";
				
			this.category = category;
			this.time = time;
		}

		public string Sender
		{
			get {return sender;}
		}
		
		public string Message
		{
			get {return message;}
		}
		
		public string[] Tags
		{
			get {return tags;}
		}
		
		public EntryCategory Category
		{
			get {return category;}
		}
		
		public DateTime Time
		{
			get {return time;}
		}
		
		public override string ToString()
		{
			string result = "<" + time.ToString("yyyy-MM-dd HH:mm:ss:fff") + "> (" + Enum.GetValues(typeof(EntryCategory)).GetValue((int)category) + ") ";
			
			if (tags != null)
			{
				foreach (string tag in tags)
					result += "[" + tag + "]";
				
				result += " ";
			}
			
			if (sender != null)
			{
				result += sender.ToString();
				result += ": ";
			}
			
			result += message;
			
			return result;
		}
	}
}
