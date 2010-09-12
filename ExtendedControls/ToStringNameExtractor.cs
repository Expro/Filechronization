
using System;

namespace ExtendedControls
{
	public class ToStringNameExtractor: IItemNameExtractor
	{
		public string ExtractName(object item)
		{
			return item.ToString();
		}
	}
}
