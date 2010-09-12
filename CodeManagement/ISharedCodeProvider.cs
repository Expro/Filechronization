/*
 *
 * User: Expro
 * Date: 2010-07-22
 * Time: 14:17
 * 
 * 
 */
using System;
using System.Security;
using CodeManagement.Definitions;

namespace CodeManagement
{
	#region Comment
	/// <summary>
	/// 	Interface for cross-domain shared code providers.
	/// </summary>
	#endregion
	public interface ISharedCodeProvider
	{
		MarshalByRefObject ProvideSharedCode(string className);
	}
}
