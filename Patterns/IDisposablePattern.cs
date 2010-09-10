/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-30
 * Time: 23:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Patterns
{
	#region Comment
	/// <summary>
	/// 	Pattern for disposable classess with unmanaged and managed resources.
	/// </summary>
	/// <remarks>
	/// 	Due to nature of interfaces, <code>CheckDisposed</code> and <code>Dispose(bool)</code> are public instead of protected.
	/// </remarks>
	#endregion
	public interface IDisposablePattern: IDisposable
	{
		void CheckDisposed();
		void Dispose(bool disposeManagedResources);
		
		bool Disposed {get;}
	}
}
