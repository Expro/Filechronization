/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-06
 * Godzina: 09:58
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Runtime.Serialization;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Set of data that will be maintained across code existance.
	/// </summary>
	#endregion
	public interface ISettings: ISerializable
	{
		#region Comment
		/// <summary>
		/// 	Version of settings. Increase whenever content of implementing class is changed.
		/// </summary>
		#endregion
		int Version {get;}
	}
}
