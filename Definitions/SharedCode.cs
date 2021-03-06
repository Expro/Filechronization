﻿/*
 *
 * User: Expro
 * Date: 2010-07-21
 * Time: 13:58
 * 
 * 
 */
using System;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Code accessible by other codes, provided as arguments for creation of other codes.
	/// </summary>
	/// <remarks>
	/// 	<code>MarshalByRefObject</code> put together manualy with <code>ICode</code> is also recognised as <code>SharedCode</code>.
	/// </remarks>
	#endregion
	public abstract class SharedCode: MarshalByRefObject, ICode
	{
		private IConfiguration configuration;
		
		protected SharedCode(IConfiguration configuration = null)
		{
			this.configuration = configuration;
		}
		
		public abstract void Dispose();
		
		public IConfiguration Configuration
		{
			get {return configuration;}
		}
	}
}
