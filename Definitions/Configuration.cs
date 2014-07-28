/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-07
 * Godzina: 01:26
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using Patterns;

namespace CodeManagement.Definitions
{
	public class Configuration: IConfiguration
	{
		private string settingsIdentifier;
		private Range<int> supportedSettingsVersions;
		private ISettings settings;
		
		protected virtual void OnSettingsChanged(ISettings oldValue, ISettings newValue)
		{
			if (SettingsChanged != null)
				SettingsChanged(this, new ValueChangedEventArgs<ISettings>(oldValue, newValue));
		}
		
		protected virtual void OnSettingsLoad(ISettings loadedValue)
		{
			if (SettingsLoaded != null)
				SettingsLoaded(this, new ValueEventArgs<ISettings>(loadedValue));
		}
		
		public Configuration(string settingsIdentifier, Range<int> supportedSettingsVersions, ISettings settings)
		{
			if (settingsIdentifier == null)
				throw new ArgumentNullException("settingsIdentifier");
			if (supportedSettingsVersions == null)
				throw new ArgumentNullException("supportedSettingsVersions");
			if (settings == null)
				throw new ArgumentNullException("settings");
			
			this.settingsIdentifier = settingsIdentifier;
			this.supportedSettingsVersions = supportedSettingsVersions;
			this.settings = settings;
		}
		
		public Configuration(string settingsIdentifier, int oldestVersion, int newestVersion, ISettings settings)
		{
			if (settingsIdentifier == null)
				throw new ArgumentNullException("settingsIdentifier");
			if (settings == null)
				throw new ArgumentNullException("settings");
			
			this.settingsIdentifier = settingsIdentifier;
			this.settings = settings;
			this.supportedSettingsVersions = new Range<int>(oldestVersion, newestVersion);
		}
		
		public void Load(ISettings loadedSettings)
		{
			if (loadedSettings == null)
				throw new ArgumentNullException("loadedSettings");
			
			if (SupportedSettingsVersions.InRange(loadedSettings.Version))
			{
				settings = loadedSettings;
				OnSettingsLoad(Settings);
			}
			else
				throw new ArgumentOutOfRangeException("loadedSettings");
		}
		
		public string SettingsIdentifier
		{
			get {return settingsIdentifier;}
		}
		
		public Range<int> SupportedSettingsVersions
		{
			get {return supportedSettingsVersions;}
		}
		
		public ISettings Settings
		{
			get {return settings;}
			set
			{
				if (value == null)
					throw new NullReferenceException();
				
				ISettings oldValue = settings;
				settings = value;
				
				OnSettingsChanged(oldValue, settings);
			}
		}		
		
		public event EventHandler<ValueEventArgs<ISettings>> SettingsLoaded;
		public event EventHandler<ValueChangedEventArgs<ISettings>> SettingsChanged;
	}
}
