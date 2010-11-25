/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-07
 * Godzina: 01:50
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Runtime.Serialization;

namespace CodeManagement.Definitions
{
	/// <summary>
	/// 	Description of AbstractSettings.
	/// </summary>
	public abstract class AbstractSettings: ISettings
	{
		private int version;
		
		public AbstractSettings(int version = 1)
		{
			this.version = version;
		}
		
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Version", Version);
		}
		
		public int Version
		{
			get {return version;}
		}
	}
}
