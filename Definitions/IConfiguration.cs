/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-06
 * Godzina: 23:53
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using Patterns;

namespace CodeManagement.Definitions
{
	#region Comment
	/// <summary>
	/// 	Data necessery to provide settings persistance of <see cref="ICode"/> objects.
	/// </summary>
	#endregion
	public interface IConfiguration
	{
		#region Comment
		/// <summary>
		/// 	Loads previously saved set of settings.
		/// </summary>
		/// <param name="settings">
		/// 	Loaded set of settings. Cannot be null, must be in range of supported settings versions.
		/// </param>
		#endregion
		void Load(ISettings settings);
		
		#region Comment
		/// <summary>
		/// 	Unique identifier, common for all versions of settings.
		/// </summary>
		#endregion
		string SettingsIdentifier {get;}
		
		#region Comment
		/// <summary>
		/// 	Supported range of settings versions.
		/// </summary>
		#endregion
		Range<int> SupportedSettingsVersions {get;}
		
		#region Comment
		/// <summary>
		/// 	Current object settings.
		/// </summary>
		/// <remarks>
		/// 	Set this property, if change of settings occures due to reconfiguration. If new settings are loaded, use <see cref="IConfiguration.Load()"/>.
		/// </remarks>
		#endregion
		ISettings Settings {get; set;}
		
		#region Comment
		/// <summary>
		/// 	Indicates change of <see cref="IConfiguration.Settings"/> property due to loading settings from external source.
		/// </summary>
		#endregion
		event EventHandler<ValueEventArgs<ISettings>> SettingsLoaded;
		
		#region Comment
		/// <summary>
		/// 	Indicates change of <see cref="IConfiguration.Settings"/> property due to reconfiguration of <see cref="ICode"/> object.
		/// </summary>
		#endregion
		event EventHandler<ValueChangedEventArgs<ISettings>> SettingsChanged;
	}
}
