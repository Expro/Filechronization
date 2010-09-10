
using System;

namespace CustomControls
{
	public class ToStringNameExtractor: IItemNameExtractor
	{
		public string ExtractName(object item)
		{
			return item.ToString();
		}
	}
}
