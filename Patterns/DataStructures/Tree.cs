/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Expro
 * Data: 2010-10-10
 * Godzina: 16:44
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;

namespace Patterns.DataStructures
{
	public class Tree<T>
	{
		private TreeNode<T> root;
		
		public TreeNode<T> Root
		{
			get {return root;}
		}
	}
}
