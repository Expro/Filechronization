/*
 *
 * User: Expro
 * Date: 2010-07-28
 * Time: 17:07
 * 
 * 
 */
using System;
using System.Security;

namespace CodeManagement
{
	public interface ICodeController
	{
		bool Load();
		bool Create();
		bool Destroy();
		bool Unload();
		bool Initialize();
		
		CodeState State
		{
			get;
		}
	}
}
