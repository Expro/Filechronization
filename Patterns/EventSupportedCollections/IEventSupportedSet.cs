/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-08
 * Time: 14:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Patterns.EventSupportedCollections
{
	public interface IEventSupportedSet<T>: ISet<T>, IEventSupportedCollection<T>
	{
		bool Remove(T item);
	}
}
