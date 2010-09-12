
using System;

namespace ExtendedControls
{
	public delegate void ItemRemoveClickedEventHandler(object sender, ItemRemoveClickedEventArgs e);
	
	#region Comment
	/// <summary>
	/// 	Description of ItemRemoveClickedEventArgs.
	/// </summary>
	#endregion
	public class ItemRemoveClickedEventArgs: EventArgs
	{
		private object item;
		
		public ItemRemoveClickedEventArgs(object item)
		{
			this.item = item;
		}
		
		public object Item
		{
			get {return item;}
		}
	}
}
