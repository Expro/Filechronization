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
	public delegate void NewSenderEventHandler(object sender, NewSenderEventArgs e);
	
	#region Comment
	/// <summary>
	/// Description of NewSenderEventArgs.
	/// </summary>
	#endregion
	public class NewSenderEventArgs: EventArgs
	{
		private object sender;
		
		public NewSenderEventArgs(object sender)
		{
			if (sender == null)
				throw new ArgumentNullException("sender");
			
			this.sender = sender;
		}
		
		public object Sender
		{
			get {return sender;}
			set {sender = value;}
		}
	}
}
