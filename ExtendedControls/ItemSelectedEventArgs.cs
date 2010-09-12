
using System;

namespace ExtendedControls
{
	public delegate void ItemSelectedEventHandler(object sender, ItemSelectedEventArgs e);
	
	#region Comment
	/// <summary>
	/// 	Item selection event handler.
	/// </summary>
	#endregion
	public class ItemSelectedEventArgs
	{
		private object item;
		
		public ItemSelectedEventArgs(object item)
		{
			this.item = item;
		}
		
		public object Item
		{
			get {return item;}
		}
	}
}
