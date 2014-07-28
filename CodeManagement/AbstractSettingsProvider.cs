/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-10
 * Godzina: 00:51
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using CodeManagement.Definitions;

namespace CodeManagement
{
	public abstract class AbstractSettingsProvider: ISettingsProvider
	{
		private IStorage storage;
		
		public AbstractSettingsProvider(IStorage storage)
		{
			if (storage == null)
				throw new ArgumentNullException("storage");
			
			this.storage = storage;
		}
		
		public void Save(ICode code)
		{
			if (code == null)
				throw new ArgumentNullException("code");
			
			if (code.Configuration != null)
				Storage.Store(SavePath(code), code.Configuration.Settings);
		}
		
		public void Load(ICode code)
		{
			if (code == null)
				throw new ArgumentNullException("code");
			
			if (code.Configuration != null)
			{
				foreach (string path in LoadPaths(code))
				{
					try
					{
						code.Configuration.Load(Storage.Restore(path) as ISettings);
						break;
					}
					catch (ArgumentNullException)
					{
						continue;
					}
				}
			}
		}
		
		public abstract string SavePath(ICode code);
		public abstract IEnumerable<string> LoadPaths(ICode code);
		
		public IStorage Storage
		{
			get {return storage;}
		}
	}
}
