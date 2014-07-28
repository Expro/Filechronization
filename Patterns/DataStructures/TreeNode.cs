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
	public struct TreeNode<T>
	{
		private TreeNode<T> parent;
		private ICollection<TreeNode<T>> children;
		private T data;
		
		public TreeNode(T data, Tree<T>.TreeNode parent = null)
		{
			this.data = data;
			this.parent = parent;
			this.children = new HashSet<TreeNode<T>>();
		}
		
		public TreeNode<T> Parent
		{
			get {return parent;}
		}
		
		public ICollection<TreeNode<T>> Children
		{
			get {return children;}
		}
		
		public T Data
		{
			get {return data;}
		}
	}
}
