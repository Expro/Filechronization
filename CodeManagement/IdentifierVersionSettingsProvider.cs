/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-10
 * Godzina: 01:04
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using CodeManagement.Definitions;

namespace CodeManagement
{
	public class IdentifierVersionSettingsProvider: AbstractSettingsProvider
	{
		public IdentifierVersionSettingsProvider(IStorage storage): base(storage)
		{
		}
		
		public override string SavePath(ICode code)
		{
			if (code == null)
				throw new ArgumentNullException("code");
			
			if (code.Configuration != null)
				return String.Format("\\{0}\\{1}", code.Configuration.SettingsIdentifier, code.Configuration.Settings.Version);
			else
				return null;
		}
		
		public override IEnumerable<string> LoadPaths(ICode code)
		{
			if (code == null)
				throw new ArgumentNullException("code");
			
			if (code.Configuration != null)
			{
				for (int i = code.Configuration.SupportedSettingsVersions.To; i >= code.Configuration.SupportedSettingsVersions.From; i--)
					yield return String.Format("\\{0}\\{1}", code.Configuration.SettingsIdentifier, i);
			}
		}
	}
}
