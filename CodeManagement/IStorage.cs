/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-07
 * Godzina: 02:04
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.IO;
using System.Runtime.Serialization;

using Patterns;

namespace CodeManagement
{
	public interface IStorage: IDisposablePattern
	{
		void Store(string path, ISerializable data);
		ISerializable Restore(string path);
	}
}
