
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CustomControls
{
	public enum CollectionDisplayMode {List, Tree};
	
	#region Comment
	/// <summary>
	/// 	Description of CollectionView.
	/// </summary>
	#endregion
	public partial class CollectionView: UserControl
	{
		#region Private Variables
		private Control currentControl;
		private CollectionDisplayMode displayMode;
		private CollectionViewItems items;
		private IItemNameExtractor nameExtractor;
		private object selectedItem;
		#endregion
		
		#region Private Methods
		private void ListViewModeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			ListView list = CurrentControl as ListView;
			if (list.FocusedItem != null)
				list.View = (View)listViewModeComboBox.SelectedIndex;
		}
		
		private void ChangeModeButton(ToolStripMenuItem clickedItem)
		{
			modeButton.Text = clickedItem.Text;
			modeButton.Image = clickedItem.Image;
		}
		
		private void ListModeClick(object sender, EventArgs e)
		{
			ChangeModeButton(listMode);
			
			DisplayMode = CollectionDisplayMode.List;
		}
		
		private void TreeModeClick(object sender, EventArgs e)
		{
			ChangeModeButton(treeMode);
			
			DisplayMode = CollectionDisplayMode.Tree;
		}
		
		private void ListItemSelectionChangerHandler(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			selectedItem = e.Item.Tag;
			OnItemSelected(e.Item.Tag);
		}
		
		private void TreeNodeMouseClickHandler(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Tag != null)
			{
				selectedItem = e.Node.Tag;
				OnItemSelected(e.Node.Tag);
			}
		}
		
		private void ContentChangedHandler(object sender, EventArgs e)
		{
			RefreshContent();
		}
		
		private void AddItemClick(object sender, EventArgs e)
		{
			OnItemAddClicked();
		}
		
		private void EnableRemoveButton(object sender, ItemSelectedEventArgs e)
		{
			removeItem.Enabled = (SelectedItem != null);
		}
		
		private void RemoveItemClick(object sender, EventArgs e)
		{
			OnItemRemoveClicked(SelectedItem);
		}
		
		private void ListLinesButtonClick(object sender, EventArgs e)
		{
			(CurrentControl as ListView).GridLines = listLinesButton.Checked;
		}
		#endregion
		
		#region Protected Methods
		protected void OnItemSelected(object item)
		{
			if (ItemSelected != null)
				ItemSelected(this, new ItemSelectedEventArgs(item));
		}
		
		protected void OnItemAddClicked()
		{
			if (ItemAddClicked != null)
				ItemAddClicked(this, new EventArgs());
		}
		
		protected void OnItemRemoveClicked(object item)
		{
			if (ItemRemoveClicked != null)
				ItemRemoveClicked(this, new ItemRemoveClickedEventArgs(item));
		}
		#endregion
		
		#region Protected Properties
		protected Control CurrentControl
		{
			get {return currentControl;}
			
			set
			{
				if (currentControl != null)
					currentControl.Parent = null;
				
				currentControl = value;
				
				if (currentControl != null)
				{
					currentControl.Parent = toolContainer.ContentPanel;
					currentControl.Dock = DockStyle.Fill;
					
					RefreshContent();
				}
			}
		}
		#endregion
		
		#region Constructors
		public CollectionView()
		{
			items = new CollectionViewItems();
			items.ContentChanged += ContentChangedHandler;
			nameExtractor = new ToStringNameExtractor();
			
			InitializeComponent();
			
			listViewModeComboBox.Items.AddRange(Enum.GetNames(typeof(View)));
			DisplayPanelVisible = true;
			ItemsPanelVisible = true;
			DisplayMode = CollectionDisplayMode.List;
			
			ItemSelected += EnableRemoveButton;
		}
		#endregion
		
		#region Public Methods
		public void RefreshContent()
		{
			ListView list;
			TreeView tree;
			
			if (!items.Contains(selectedItem))
				SelectedItem = null;
			
			switch (DisplayMode)
			{
				case CollectionDisplayMode.List:
					list = CurrentControl as ListView;
					
					list.BeginUpdate();
					list.Items.Clear();
					
					if (items != null)
					{
						foreach (object item in items)
							list.Items.Add(NameExtractor.ExtractName(item)).Tag = item;
					}
					
					list.EndUpdate();
					list.Refresh();
					
					if (selectedItem != null)
						list.FocusedItem = list.Items[(NameExtractor.ExtractName(selectedItem))];
					
					break;
					
				case CollectionDisplayMode.Tree:
					tree = CurrentControl as TreeView;
					
					tree.BeginUpdate();
					tree.Nodes.Clear();
					
					if (items != null)
					{
						foreach (object item in items)
							tree.Nodes.Add(nameExtractor.ExtractName(item)).Tag = item;
					}
					
					tree.EndUpdate();
					
					break;
			}
			
			Refresh();
		}
		#endregion
		
		#region Public Properties
		public CollectionDisplayMode DisplayMode
		{
			get {return displayMode;}
			
			set
			{
				ListView list;
				TreeView tree;
				
				displayMode = value;
				
				foreach (Control control in toolContainer.TopToolStripPanel.Controls)
				{
					if (control is ToolStrip)
					{
						if (control != modeStrip)
							control.Visible = false;
					}
				}
				
				switch (displayMode)
				{
					case CollectionDisplayMode.List:
						list = new ListView();
						list.View = View.Details;
						list.FullRowSelect = true;
						list.HeaderStyle = ColumnHeaderStyle.None;
						list.ItemSelectionChanged += ListItemSelectionChangerHandler;
							
						CurrentControl = list;
						
						list.Columns.Add("Items", 100);
						list.Resize += delegate (object sender, EventArgs e) {list.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);};
						
						listStrip.Visible = true;
						listViewModeComboBox.SelectedIndex = listViewModeComboBox.Items.IndexOf(list.View.ToString());
						
						break;
					case CollectionDisplayMode.Tree:
						tree = new TreeView();
						tree.NodeMouseClick += TreeNodeMouseClickHandler;
						
						CurrentControl = tree;
						
						break;
				}
			}
		}
		
		public CollectionViewItems Items
		{
			get {return items;}
		}
		
		public IItemNameExtractor NameExtractor
		{
			get {return nameExtractor;}
			set
			{
				if (value == null)
					throw new NullReferenceException("Name Extractor must exist.");
				else
					nameExtractor = value;
				
				RefreshContent();
			}
		}
		
		public bool ItemsPanelVisible
		{
			get {return itemsStrip.Visible;}
			set {itemsStrip.Visible = value;}
		}
		
		public bool DisplayPanelVisible
		{
			get {return modeStrip.Visible;}
			
			set
			{
				modeStrip.Visible = value;
				listStrip.Visible = value && DisplayMode.Equals(CollectionDisplayMode.List);
			}
		}
		
		public object SelectedItem
		{
			get {return selectedItem;}
			
			set
			{
				selectedItem = value;
				OnItemSelected(SelectedItem);
			}
		}
		#endregion
		
		#region Events
		public event ItemSelectedEventHandler ItemSelected;
		public event EventHandler ItemAddClicked;
		public event ItemRemoveClickedEventHandler ItemRemoveClicked;
		#endregion
	}
}
