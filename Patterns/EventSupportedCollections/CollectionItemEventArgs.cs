/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-08
 * Time: 13:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Patterns.EventSupportedCollections
{
	public class CollectionItemEventArgs<T>: EventArgs
	{
		private T item;
		
		public CollectionItemEventArgs(T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			
			this.item = item;
		}
		
		public T Item
		{
			get {return item;}
		}
	}
}
