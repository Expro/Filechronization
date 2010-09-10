/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-08
 * Time: 14:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace Patterns.EventSupportedCollections
{
	public class EventSupportedHashSet<T>: HashSet<T>, IEventSupportedSet<T>
	{
		protected virtual void OnItemAdded(CollectionItemEventArgs<T> e)
		{
			if (ItemAdded != null)
				ItemAdded(this, e);
		}
		
		protected virtual void OnItemRemoved(CollectionItemEventArgs<T> e)
		{
			if (ItemRemoved != null)
				ItemRemoved(this, e);
		}
		
		public EventSupportedHashSet()
		{
		}
		
		public bool IsReadOnly
		{
			get {return false;}
		}
		
		public new bool Add(T item)
		{
			bool result = base.Add(item);
			
			OnItemAdded(new CollectionItemEventArgs<T>(item));
			
			return result;
		}
		
		public new bool Remove(T item)
		{
			bool result = base.Remove(item);
			
			OnItemRemoved(new CollectionItemEventArgs<T>(item));
			
			return result;
		}
		
		public event EventHandler<CollectionItemEventArgs<T>> ItemAdded;
		public event EventHandler<CollectionItemEventArgs<T>> ItemRemoved;
	}
}
