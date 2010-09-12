
using System;

namespace ExtendedControls
{
	#region Comment
	/// <summary>
	/// 	Description of IItemNameExtractor.
	/// </summary>
	#endregion
	public interface IItemNameExtractor
	{
		string ExtractName(object item);
	}
}
