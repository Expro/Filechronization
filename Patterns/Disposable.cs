/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-30
 * Time: 23:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Patterns
{
	public abstract class Disposable: IDisposablePattern
	{
		private bool disposed;
		
		~Disposable()
		{
			Dispose(false);
		}
		
		public Disposable()
		{
			disposed = false;
		}
		
		public bool Disposed
		{
			get {return disposed;}
		}
		
		public void CheckDisposed()
		{
			if (disposed)
				throw new ObjectDisposedException(ToString());
		}
		
		public virtual void Dispose(bool disposeManagedResources)
		{
			disposed = true;
		}
		
		public void Dispose()
		{
			if (!Disposed)
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}
	}
}
