/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-08
 * Time: 13:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Patterns.EventSupportedCollections
{
	public interface IEventSupportedCollection<T>: ICollection<T>
	{
		event EventHandler<CollectionItemEventArgs<T>> ItemAdded;
		event EventHandler<CollectionItemEventArgs<T>> ItemRemoved;
	}
}
