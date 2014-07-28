/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-07
 * Godzina: 00:16
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace Patterns
{
	/// <summary>
	/// 	Description of ValueEventArgs.
	/// </summary>
	public class ValueEventArgs<T>: EventArgs
	{
		private T value;
		
		public ValueEventArgs(T value)
		{
			this.value = value;
		}
		
		public T Value
		{
			get {return value;}
		}
	}
}
