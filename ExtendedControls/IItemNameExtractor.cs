
using System;

namespace CustomControls
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
