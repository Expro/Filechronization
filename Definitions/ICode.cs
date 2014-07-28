/*
 *
 * User: Expro
 * Date: 2010-07-21
 * Time: 13:57
 * 
 * 
 */
using System;
using Patterns;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Marks class as code managable by code manager.
	/// </summary>
	#endregion
	public interface ICode: IDisposable
	{
		IConfiguration Configuration {get;}
	}
}
