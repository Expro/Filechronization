
using System;
using System.Collections.Generic;

namespace ExtendedControls
{
	#region Comment
	/// <summary>
	/// 	Description of CollectionViewItems.
	/// </summary>
	#endregion
	public class CollectionViewItems: ICollection<object>
	{
		#region Private Variables
		private HashSet<object> items;
		#endregion
		
		#region Private Properties
		private ICollection<object> Collection
		{
			get {return items;}
		}
		#endregion
		
		#region Protected Methods
		protected virtual void OnContentChanged()
		{
			if (ContentChanged != null)
				ContentChanged(this, new EventArgs());
		}
		#endregion
		
		#region Constuctors
		internal CollectionViewItems()
		{
			items = new HashSet<object>();
		}
		#endregion	
		
		#region Public Methods
		public void Add(object item)
		{
			Collection.Add(item);
			OnContentChanged();
		}
		
		public void Clear()
		{
			Collection.Clear();
			OnContentChanged();
		}
		
		public bool Contains(object item)
		{
			return Collection.Contains(item);
		}
		
		public void CopyTo(object[] array, int arrayIndex)
		{
			Collection.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(object item)
		{
			bool result = Collection.Remove(item);
			OnContentChanged();
			return result;
		}
		
		public IEnumerator<object> GetEnumerator()
		{
			return Collection.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Collection.GetEnumerator();
		}
		#endregion
		
		#region Public Properies
		public int Count
		{
			get
			{
				return Collection.Count;
			}
		}
		
		public bool IsReadOnly
		{
			get
			{
				return Collection.IsReadOnly;
			}
		}
		#endregion
		
		#region Events
		public event EventHandler ContentChanged;
		#endregion
	}
}
