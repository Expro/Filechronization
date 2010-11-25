/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-06
 * Godzina: 09:57
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using CodeManagement.Definitions;

namespace CodeManagement
{
	#region Comment
	/// <summary>
	/// 	Description of ISettingsProvider.
	/// </summary>
	#endregion
	public interface ISettingsProvider
	{
		void Load(ICode code);
		void Save(ICode code);
		string SavePath(ICode code);
		IEnumerable<string> LoadPaths(ICode code);
		
		IStorage Storage {get;}
	}
}
