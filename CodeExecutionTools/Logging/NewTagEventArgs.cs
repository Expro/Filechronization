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
	public delegate void NewTagEventHandler(object sender, NewTagEventArgs e);
	
	#region Comment
	/// <summary>
	/// Description of NewTagEventArgs.
	/// </summary>
	#endregion
	public class NewTagEventArgs: EventArgs
	{
		private string tag;
		
		public NewTagEventArgs(string tag)
		{
			if (tag == null)
				throw new ArgumentNullException("tag");
			
			this.tag = tag;
		}
		
		public string Tag
		{
			get {return tag;}
			set {tag = value;}
		}
	}
}