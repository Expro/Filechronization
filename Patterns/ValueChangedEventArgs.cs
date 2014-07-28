/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-06
 * Godzina: 23:43
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace Patterns
{
	public class ValueChangedEventArgs<T>: EventArgs
	{
		private T oldValue;
		private T newValue;
		
		public ValueChangedEventArgs(T oldValue, T newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		
		public T OldValue
		{
			get {return oldValue;}
		}
		
		public T NewValue
		{
			get {return newValue;}
		}
	}
}
