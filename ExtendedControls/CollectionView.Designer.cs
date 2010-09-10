
namespace CustomControls
{
	partial class CollectionView
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectionView));
			this.toolContainer = new System.Windows.Forms.ToolStripContainer();
			this.itemsStrip = new System.Windows.Forms.ToolStrip();
			this.addItem = new System.Windows.Forms.ToolStripButton();
			this.removeItem = new System.Windows.Forms.ToolStripButton();
			this.modeStrip = new System.Windows.Forms.ToolStrip();
			this.modeButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.listMode = new System.Windows.Forms.ToolStripMenuItem();
			this.treeMode = new System.Windows.Forms.ToolStripMenuItem();
			this.listStrip = new System.Windows.Forms.ToolStrip();
			this.listViewModeLabel = new System.Windows.Forms.ToolStripLabel();
			this.listViewModeComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.listLinesButton = new System.Windows.Forms.ToolStripButton();
			this.toolContainer.RightToolStripPanel.SuspendLayout();
			this.toolContainer.TopToolStripPanel.SuspendLayout();
			this.toolContainer.SuspendLayout();
			this.itemsStrip.SuspendLayout();
			this.modeStrip.SuspendLayout();
			this.listStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolContainer
			// 
			// 
			// toolContainer.ContentPanel
			// 
			this.toolContainer.ContentPanel.Size = new System.Drawing.Size(477, 390);
			this.toolContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolContainer.Location = new System.Drawing.Point(0, 0);
			this.toolContainer.Name = "toolContainer";
			// 
			// toolContainer.RightToolStripPanel
			// 
			this.toolContainer.RightToolStripPanel.Controls.Add(this.itemsStrip);
			this.toolContainer.Size = new System.Drawing.Size(501, 415);
			this.toolContainer.TabIndex = 0;
			this.toolContainer.Text = "toolStripContainer1";
			// 
			// toolContainer.TopToolStripPanel
			// 
			this.toolContainer.TopToolStripPanel.Controls.Add(this.modeStrip);
			this.toolContainer.TopToolStripPanel.Controls.Add(this.listStrip);
			// 
			// itemsStrip
			// 
			this.itemsStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.itemsStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addItem,
									this.removeItem});
			this.itemsStrip.Location = new System.Drawing.Point(0, 3);
			this.itemsStrip.Name = "itemsStrip";
			this.itemsStrip.Size = new System.Drawing.Size(24, 57);
			this.itemsStrip.TabIndex = 0;
			// 
			// addItem
			// 
			this.addItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.addItem.Image = ((System.Drawing.Image)(resources.GetObject("addItem.Image")));
			this.addItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.addItem.Name = "addItem";
			this.addItem.Size = new System.Drawing.Size(22, 20);
			this.addItem.Text = "Add";
			this.addItem.ToolTipText = "Adds new item.";
			this.addItem.Click += new System.EventHandler(this.AddItemClick);
			// 
			// removeItem
			// 
			this.removeItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.removeItem.Enabled = false;
			this.removeItem.Image = ((System.Drawing.Image)(resources.GetObject("removeItem.Image")));
			this.removeItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.removeItem.Name = "removeItem";
			this.removeItem.Size = new System.Drawing.Size(22, 20);
			this.removeItem.Text = "Remove";
			this.removeItem.ToolTipText = "Removes selected item.";
			this.removeItem.Click += new System.EventHandler(this.RemoveItemClick);
			// 
			// modeStrip
			// 
			this.modeStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.modeStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.modeButton});
			this.modeStrip.Location = new System.Drawing.Point(3, 0);
			this.modeStrip.Name = "modeStrip";
			this.modeStrip.Size = new System.Drawing.Size(61, 25);
			this.modeStrip.TabIndex = 0;
			// 
			// modeButton
			// 
			this.modeButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.listMode,
									this.treeMode});
			this.modeButton.Image = ((System.Drawing.Image)(resources.GetObject("modeButton.Image")));
			this.modeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.modeButton.Name = "modeButton";
			this.modeButton.Size = new System.Drawing.Size(49, 22);
			this.modeButton.Text = "List";
			// 
			// listMode
			// 
			this.listMode.Image = ((System.Drawing.Image)(resources.GetObject("listMode.Image")));
			this.listMode.Name = "listMode";
			this.listMode.Size = new System.Drawing.Size(95, 22);
			this.listMode.Text = "List";
			this.listMode.Click += new System.EventHandler(this.ListModeClick);
			// 
			// treeMode
			// 
			this.treeMode.Image = ((System.Drawing.Image)(resources.GetObject("treeMode.Image")));
			this.treeMode.Name = "treeMode";
			this.treeMode.Size = new System.Drawing.Size(95, 22);
			this.treeMode.Text = "Tree";
			this.treeMode.Click += new System.EventHandler(this.TreeModeClick);
			// 
			// listStrip
			// 
			this.listStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.listStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.listViewModeLabel,
									this.listViewModeComboBox,
									this.listLinesButton});
			this.listStrip.Location = new System.Drawing.Point(64, 0);
			this.listStrip.Name = "listStrip";
			this.listStrip.Size = new System.Drawing.Size(234, 25);
			this.listStrip.TabIndex = 1;
			// 
			// listViewModeLabel
			// 
			this.listViewModeLabel.Name = "listViewModeLabel";
			this.listViewModeLabel.Size = new System.Drawing.Size(52, 22);
			this.listViewModeLabel.Text = "View Mode:";
			// 
			// listViewModeComboBox
			// 
			this.listViewModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listViewModeComboBox.Name = "listViewModeComboBox";
			this.listViewModeComboBox.Size = new System.Drawing.Size(121, 25);
			this.listViewModeComboBox.SelectedIndexChanged += new System.EventHandler(this.ListViewModeComboBoxSelectedIndexChanged);
			// 
			// listLinesButton
			// 
			this.listLinesButton.CheckOnClick = true;
			this.listLinesButton.Image = ((System.Drawing.Image)(resources.GetObject("listLinesButton.Image")));
			this.listLinesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.listLinesButton.Name = "listLinesButton";
			this.listLinesButton.Size = new System.Drawing.Size(47, 22);
			this.listLinesButton.Text = "Lines";
			this.listLinesButton.ToolTipText = "Grid lines for Detail view mode.";
			this.listLinesButton.Click += new System.EventHandler(this.ListLinesButtonClick);
			// 
			// CollectionView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.toolContainer);
			this.Name = "CollectionView";
			this.Size = new System.Drawing.Size(501, 415);
			this.toolContainer.RightToolStripPanel.ResumeLayout(false);
			this.toolContainer.RightToolStripPanel.PerformLayout();
			this.toolContainer.TopToolStripPanel.ResumeLayout(false);
			this.toolContainer.TopToolStripPanel.PerformLayout();
			this.toolContainer.ResumeLayout(false);
			this.toolContainer.PerformLayout();
			this.itemsStrip.ResumeLayout(false);
			this.itemsStrip.PerformLayout();
			this.modeStrip.ResumeLayout(false);
			this.modeStrip.PerformLayout();
			this.listStrip.ResumeLayout(false);
			this.listStrip.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ToolStripButton listLinesButton;
		private System.Windows.Forms.ToolStrip itemsStrip;
		private System.Windows.Forms.ToolStripButton removeItem;
		private System.Windows.Forms.ToolStripButton addItem;
		private System.Windows.Forms.ToolStripComboBox listViewModeComboBox;
		private System.Windows.Forms.ToolStripLabel listViewModeLabel;
		private System.Windows.Forms.ToolStrip listStrip;
		private System.Windows.Forms.ToolStripMenuItem treeMode;
		private System.Windows.Forms.ToolStripMenuItem listMode;
		private System.Windows.Forms.ToolStripDropDownButton modeButton;
		private System.Windows.Forms.ToolStrip modeStrip;
		private System.Windows.Forms.ToolStripContainer toolContainer;
	}
}
