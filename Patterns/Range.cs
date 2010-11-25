/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-06
 * Godzina: 11:02
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace Patterns
{
	/// <summary>
	/// 	Description of Range.
	/// </summary>
	public class Range<T> where T: IComparable<T>
	{
		private T from;
		private T to;
		
		public Range(T from, T to)
		{
			if (from == null)
				throw new ArgumentNullException("from");
			if (to == null)
				throw new ArgumentNullException("to");
			if (from.CompareTo(to) > 0)
				throw new ArgumentException("From cannot be greater than to");
			
			this.from = from;
			this.to = to;
		}
		
		public bool InRange(T checkedValue)
		{
			return (checkedValue.CompareTo(from) >= 0) && (checkedValue.CompareTo(to) <= 0);
		}
		
		public T From
		{
			get {return from;}
		}
		
		public T To
		{
			get {return to;}
		}
	}
}
