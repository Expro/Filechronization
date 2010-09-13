/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-31
 * Time: 15:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace LogExplorer
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menu = new System.Windows.Forms.MenuStrip();
			this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.openMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.closeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.entryMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.copyMessageMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.copyAllMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.propertiesPanel = new System.Windows.Forms.Panel();
			this.propertiesTabs = new System.Windows.Forms.TabControl();
			this.typesTab = new System.Windows.Forms.TabPage();
			this.errorsDescription = new System.Windows.Forms.Label();
			this.typeErrors = new System.Windows.Forms.CheckBox();
			this.logImages = new System.Windows.Forms.ImageList(this.components);
			this.warningsDescription = new System.Windows.Forms.Label();
			this.typeWarnings = new System.Windows.Forms.CheckBox();
			this.informationsDescription = new System.Windows.Forms.Label();
			this.typeInformations = new System.Windows.Forms.CheckBox();
			this.tagsTab = new System.Windows.Forms.TabPage();
			this.tags = new System.Windows.Forms.CheckedListBox();
			this.tagsDescription = new System.Windows.Forms.Label();
			this.sendersTab = new System.Windows.Forms.TabPage();
			this.senders = new System.Windows.Forms.CheckedListBox();
			this.sendersDescription = new System.Windows.Forms.Label();
			this.tabImages = new System.Windows.Forms.ImageList(this.components);
			this.propertiesPanelHeader = new System.Windows.Forms.Panel();
			this.propertiesPanelHeaderLabel = new System.Windows.Forms.Label();
			this.logPanelHeader = new System.Windows.Forms.Panel();
			this.logPanelHeaderLabel = new System.Windows.Forms.Label();
			this.logTree = new System.Windows.Forms.TreeView();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.menu.SuspendLayout();
			this.propertiesPanel.SuspendLayout();
			this.propertiesTabs.SuspendLayout();
			this.typesTab.SuspendLayout();
			this.tagsTab.SuspendLayout();
			this.sendersTab.SuspendLayout();
			this.propertiesPanelHeader.SuspendLayout();
			this.logPanelHeader.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// menu
			// 
			this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileMenu,
									this.entryMenu});
			this.menu.Location = new System.Drawing.Point(0, 0);
			this.menu.Name = "menu";
			this.menu.Size = new System.Drawing.Size(920, 24);
			this.menu.TabIndex = 0;
			this.menu.Text = "menuStrip1";
			// 
			// fileMenu
			// 
			this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.openMenu,
									this.toolStripSeparator1,
									this.closeMenu});
			this.fileMenu.Image = ((System.Drawing.Image)(resources.GetObject("fileMenu.Image")));
			this.fileMenu.Name = "fileMenu";
			this.fileMenu.Size = new System.Drawing.Size(48, 20);
			this.fileMenu.Text = "File";
			// 
			// openMenu
			// 
			this.openMenu.Image = ((System.Drawing.Image)(resources.GetObject("openMenu.Image")));
			this.openMenu.Name = "openMenu";
			this.openMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openMenu.Size = new System.Drawing.Size(140, 22);
			this.openMenu.Text = "Open...";
			this.openMenu.Click += new System.EventHandler(this.OpenMenuClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(137, 6);
			// 
			// closeMenu
			// 
			this.closeMenu.Image = ((System.Drawing.Image)(resources.GetObject("closeMenu.Image")));
			this.closeMenu.Name = "closeMenu";
			this.closeMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.closeMenu.Size = new System.Drawing.Size(140, 22);
			this.closeMenu.Text = "Close";
			this.closeMenu.Click += new System.EventHandler(this.CloseMenuClick);
			// 
			// entryMenu
			// 
			this.entryMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.copyMessageMenu,
									this.copyAllMenu});
			this.entryMenu.Image = ((System.Drawing.Image)(resources.GetObject("entryMenu.Image")));
			this.entryMenu.Name = "entryMenu";
			this.entryMenu.Size = new System.Drawing.Size(56, 20);
			this.entryMenu.Text = "Entry";
			// 
			// copyMessageMenu
			// 
			this.copyMessageMenu.Enabled = false;
			this.copyMessageMenu.Image = ((System.Drawing.Image)(resources.GetObject("copyMessageMenu.Image")));
			this.copyMessageMenu.Name = "copyMessageMenu";
			this.copyMessageMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyMessageMenu.Size = new System.Drawing.Size(172, 22);
			this.copyMessageMenu.Text = "Copy Message";
			this.copyMessageMenu.Click += new System.EventHandler(this.CopyMessageMenuClick);
			// 
			// copyAllMenu
			// 
			this.copyAllMenu.Enabled = false;
			this.copyAllMenu.Image = ((System.Drawing.Image)(resources.GetObject("copyAllMenu.Image")));
			this.copyAllMenu.Name = "copyAllMenu";
			this.copyAllMenu.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
									| System.Windows.Forms.Keys.C)));
			this.copyAllMenu.Size = new System.Drawing.Size(172, 22);
			this.copyAllMenu.Text = "Copy All";
			this.copyAllMenu.Click += new System.EventHandler(this.CopyAllMenuClick);
			// 
			// propertiesPanel
			// 
			this.propertiesPanel.Controls.Add(this.propertiesTabs);
			this.propertiesPanel.Controls.Add(this.propertiesPanelHeader);
			this.propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertiesPanel.Location = new System.Drawing.Point(0, 0);
			this.propertiesPanel.Name = "propertiesPanel";
			this.propertiesPanel.Size = new System.Drawing.Size(246, 504);
			this.propertiesPanel.TabIndex = 1;
			// 
			// propertiesTabs
			// 
			this.propertiesTabs.Controls.Add(this.typesTab);
			this.propertiesTabs.Controls.Add(this.tagsTab);
			this.propertiesTabs.Controls.Add(this.sendersTab);
			this.propertiesTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertiesTabs.ImageList = this.tabImages;
			this.propertiesTabs.Location = new System.Drawing.Point(0, 20);
			this.propertiesTabs.Name = "propertiesTabs";
			this.propertiesTabs.SelectedIndex = 0;
			this.propertiesTabs.Size = new System.Drawing.Size(246, 484);
			this.propertiesTabs.TabIndex = 1;
			// 
			// typesTab
			// 
			this.typesTab.Controls.Add(this.errorsDescription);
			this.typesTab.Controls.Add(this.typeErrors);
			this.typesTab.Controls.Add(this.warningsDescription);
			this.typesTab.Controls.Add(this.typeWarnings);
			this.typesTab.Controls.Add(this.informationsDescription);
			this.typesTab.Controls.Add(this.typeInformations);
			this.typesTab.ImageKey = "types.png";
			this.typesTab.Location = new System.Drawing.Point(4, 23);
			this.typesTab.Name = "typesTab";
			this.typesTab.Padding = new System.Windows.Forms.Padding(3);
			this.typesTab.Size = new System.Drawing.Size(238, 457);
			this.typesTab.TabIndex = 0;
			this.typesTab.Text = "Types";
			this.typesTab.UseVisualStyleBackColor = true;
			// 
			// errorsDescription
			// 
			this.errorsDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.errorsDescription.Location = new System.Drawing.Point(22, 182);
			this.errorsDescription.Name = "errorsDescription";
			this.errorsDescription.Size = new System.Drawing.Size(208, 40);
			this.errorsDescription.TabIndex = 5;
			this.errorsDescription.Text = "Serious, potentialy dangerous situations that occured during application executio" +
			"n.";
			// 
			// typeErrors
			// 
			this.typeErrors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.typeErrors.Checked = true;
			this.typeErrors.CheckState = System.Windows.Forms.CheckState.Checked;
			this.typeErrors.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.typeErrors.ImageKey = "errors.png";
			this.typeErrors.ImageList = this.logImages;
			this.typeErrors.Location = new System.Drawing.Point(6, 155);
			this.typeErrors.Name = "typeErrors";
			this.typeErrors.Size = new System.Drawing.Size(224, 24);
			this.typeErrors.TabIndex = 4;
			this.typeErrors.Text = "Errors";
			this.typeErrors.UseVisualStyleBackColor = true;
			this.typeErrors.CheckedChanged += new System.EventHandler(this.TypeErrorsCheckedChanged);
			// 
			// logImages
			// 
			this.logImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("logImages.ImageStream")));
			this.logImages.TransparentColor = System.Drawing.Color.Transparent;
			this.logImages.Images.SetKeyName(0, "errors.png");
			this.logImages.Images.SetKeyName(1, "information.png");
			this.logImages.Images.SetKeyName(2, "warnings.png");
			this.logImages.Images.SetKeyName(3, "selected.png");
			// 
			// warningsDescription
			// 
			this.warningsDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.warningsDescription.Location = new System.Drawing.Point(22, 108);
			this.warningsDescription.Name = "warningsDescription";
			this.warningsDescription.Size = new System.Drawing.Size(208, 32);
			this.warningsDescription.TabIndex = 3;
			this.warningsDescription.Text = "Informations about abnormal, non-critical errors and behaviors of application.";
			// 
			// typeWarnings
			// 
			this.typeWarnings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.typeWarnings.Checked = true;
			this.typeWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
			this.typeWarnings.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.typeWarnings.ImageKey = "warnings.png";
			this.typeWarnings.ImageList = this.logImages;
			this.typeWarnings.Location = new System.Drawing.Point(6, 81);
			this.typeWarnings.Name = "typeWarnings";
			this.typeWarnings.Size = new System.Drawing.Size(224, 24);
			this.typeWarnings.TabIndex = 2;
			this.typeWarnings.Text = "Warnings";
			this.typeWarnings.UseVisualStyleBackColor = true;
			this.typeWarnings.CheckedChanged += new System.EventHandler(this.TypeWarningsCheckedChanged);
			// 
			// informationsDescription
			// 
			this.informationsDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.informationsDescription.Location = new System.Drawing.Point(22, 33);
			this.informationsDescription.Name = "informationsDescription";
			this.informationsDescription.Size = new System.Drawing.Size(208, 45);
			this.informationsDescription.TabIndex = 1;
			this.informationsDescription.Text = "Messages describing current activities of application and loaded addins.";
			// 
			// typeInformations
			// 
			this.typeInformations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.typeInformations.Checked = true;
			this.typeInformations.CheckState = System.Windows.Forms.CheckState.Checked;
			this.typeInformations.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.typeInformations.ImageKey = "information.png";
			this.typeInformations.ImageList = this.logImages;
			this.typeInformations.Location = new System.Drawing.Point(6, 6);
			this.typeInformations.Name = "typeInformations";
			this.typeInformations.Size = new System.Drawing.Size(224, 24);
			this.typeInformations.TabIndex = 0;
			this.typeInformations.Text = "Informations";
			this.typeInformations.UseVisualStyleBackColor = true;
			this.typeInformations.CheckedChanged += new System.EventHandler(this.TypeInformationsCheckedChanged);
			// 
			// tagsTab
			// 
			this.tagsTab.Controls.Add(this.tags);
			this.tagsTab.Controls.Add(this.tagsDescription);
			this.tagsTab.ImageKey = "tags.png";
			this.tagsTab.Location = new System.Drawing.Point(4, 23);
			this.tagsTab.Name = "tagsTab";
			this.tagsTab.Padding = new System.Windows.Forms.Padding(3);
			this.tagsTab.Size = new System.Drawing.Size(238, 457);
			this.tagsTab.TabIndex = 1;
			this.tagsTab.Text = "Tags";
			this.tagsTab.UseVisualStyleBackColor = true;
			// 
			// tags
			// 
			this.tags.CheckOnClick = true;
			this.tags.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tags.FormattingEnabled = true;
			this.tags.HorizontalScrollbar = true;
			this.tags.Location = new System.Drawing.Point(3, 43);
			this.tags.Name = "tags";
			this.tags.Size = new System.Drawing.Size(232, 411);
			this.tags.TabIndex = 1;
			this.tags.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.TagsItemCheck);
			// 
			// tagsDescription
			// 
			this.tagsDescription.Dock = System.Windows.Forms.DockStyle.Top;
			this.tagsDescription.Location = new System.Drawing.Point(3, 3);
			this.tagsDescription.Name = "tagsDescription";
			this.tagsDescription.Size = new System.Drawing.Size(232, 40);
			this.tagsDescription.TabIndex = 0;
			this.tagsDescription.Text = "Each message can be associated with set of tags, describing nature of event conta" +
			"ined by log entry.";
			// 
			// sendersTab
			// 
			this.sendersTab.Controls.Add(this.senders);
			this.sendersTab.Controls.Add(this.sendersDescription);
			this.sendersTab.ImageKey = "senders.png";
			this.sendersTab.Location = new System.Drawing.Point(4, 23);
			this.sendersTab.Name = "sendersTab";
			this.sendersTab.Padding = new System.Windows.Forms.Padding(3);
			this.sendersTab.Size = new System.Drawing.Size(238, 457);
			this.sendersTab.TabIndex = 2;
			this.sendersTab.Text = "Senders";
			this.sendersTab.UseVisualStyleBackColor = true;
			// 
			// senders
			// 
			this.senders.CheckOnClick = true;
			this.senders.Dock = System.Windows.Forms.DockStyle.Fill;
			this.senders.FormattingEnabled = true;
			this.senders.HorizontalScrollbar = true;
			this.senders.Location = new System.Drawing.Point(3, 43);
			this.senders.Name = "senders";
			this.senders.Size = new System.Drawing.Size(232, 411);
			this.senders.TabIndex = 1;
			this.senders.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.SendersItemCheck);
			// 
			// sendersDescription
			// 
			this.sendersDescription.Dock = System.Windows.Forms.DockStyle.Top;
			this.sendersDescription.Location = new System.Drawing.Point(3, 3);
			this.sendersDescription.Name = "sendersDescription";
			this.sendersDescription.Size = new System.Drawing.Size(232, 40);
			this.sendersDescription.TabIndex = 0;
			this.sendersDescription.Text = "Messages are usually signed by object, that is source of event described by log e" +
			"vent.";
			// 
			// tabImages
			// 
			this.tabImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tabImages.ImageStream")));
			this.tabImages.TransparentColor = System.Drawing.Color.Transparent;
			this.tabImages.Images.SetKeyName(0, "senders.png");
			this.tabImages.Images.SetKeyName(1, "tags.png");
			this.tabImages.Images.SetKeyName(2, "types.png");
			// 
			// propertiesPanelHeader
			// 
			this.propertiesPanelHeader.BackColor = System.Drawing.Color.DarkBlue;
			this.propertiesPanelHeader.Controls.Add(this.propertiesPanelHeaderLabel);
			this.propertiesPanelHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.propertiesPanelHeader.Location = new System.Drawing.Point(0, 0);
			this.propertiesPanelHeader.Name = "propertiesPanelHeader";
			this.propertiesPanelHeader.Size = new System.Drawing.Size(246, 20);
			this.propertiesPanelHeader.TabIndex = 0;
			// 
			// propertiesPanelHeaderLabel
			// 
			this.propertiesPanelHeaderLabel.BackColor = System.Drawing.Color.ForestGreen;
			this.propertiesPanelHeaderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertiesPanelHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.propertiesPanelHeaderLabel.ForeColor = System.Drawing.Color.White;
			this.propertiesPanelHeaderLabel.Location = new System.Drawing.Point(0, 0);
			this.propertiesPanelHeaderLabel.Name = "propertiesPanelHeaderLabel";
			this.propertiesPanelHeaderLabel.Size = new System.Drawing.Size(246, 20);
			this.propertiesPanelHeaderLabel.TabIndex = 0;
			this.propertiesPanelHeaderLabel.Text = "Properties";
			this.propertiesPanelHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// logPanelHeader
			// 
			this.logPanelHeader.BackColor = System.Drawing.Color.DarkBlue;
			this.logPanelHeader.Controls.Add(this.logPanelHeaderLabel);
			this.logPanelHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.logPanelHeader.Location = new System.Drawing.Point(0, 0);
			this.logPanelHeader.Name = "logPanelHeader";
			this.logPanelHeader.Size = new System.Drawing.Size(662, 20);
			this.logPanelHeader.TabIndex = 2;
			// 
			// logPanelHeaderLabel
			// 
			this.logPanelHeaderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logPanelHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.logPanelHeaderLabel.ForeColor = System.Drawing.Color.White;
			this.logPanelHeaderLabel.Location = new System.Drawing.Point(0, 0);
			this.logPanelHeaderLabel.Name = "logPanelHeaderLabel";
			this.logPanelHeaderLabel.Size = new System.Drawing.Size(662, 20);
			this.logPanelHeaderLabel.TabIndex = 1;
			this.logPanelHeaderLabel.Text = "Log";
			this.logPanelHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// logTree
			// 
			this.logTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
			this.logTree.FullRowSelect = true;
			this.logTree.ImageIndex = 0;
			this.logTree.ImageList = this.logImages;
			this.logTree.Location = new System.Drawing.Point(0, 20);
			this.logTree.Name = "logTree";
			this.logTree.SelectedImageKey = "selected.png";
			this.logTree.Size = new System.Drawing.Size(662, 484);
			this.logTree.TabIndex = 3;
			this.logTree.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.LogTreeDrawNode);
			this.logTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.LogTreeAfterSelect);
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "*.log";
			this.openFileDialog.Filter = "Filechronization Log (*.log)|*.log";
			// 
			// splitContainer
			// 
			this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 24);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.logTree);
			this.splitContainer.Panel1.Controls.Add(this.logPanelHeader);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.propertiesPanel);
			this.splitContainer.Size = new System.Drawing.Size(920, 508);
			this.splitContainer.SplitterDistance = 666;
			this.splitContainer.TabIndex = 4;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(920, 532);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.menu);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menu;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Log Explorer";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.menu.ResumeLayout(false);
			this.menu.PerformLayout();
			this.propertiesPanel.ResumeLayout(false);
			this.propertiesTabs.ResumeLayout(false);
			this.typesTab.ResumeLayout(false);
			this.tagsTab.ResumeLayout(false);
			this.sendersTab.ResumeLayout(false);
			this.propertiesPanelHeader.ResumeLayout(false);
			this.logPanelHeader.ResumeLayout(false);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.ToolStripMenuItem copyAllMenu;
		private System.Windows.Forms.ToolStripMenuItem copyMessageMenu;
		private System.Windows.Forms.ToolStripMenuItem entryMenu;
		private System.Windows.Forms.ToolStripMenuItem fileMenu;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ImageList logImages;
		private System.Windows.Forms.TreeView logTree;
		private System.Windows.Forms.ImageList tabImages;
		private System.Windows.Forms.Label sendersDescription;
		private System.Windows.Forms.Label tagsDescription;
		private System.Windows.Forms.Label informationsDescription;
		private System.Windows.Forms.Label warningsDescription;
		private System.Windows.Forms.Label errorsDescription;
		private System.Windows.Forms.TabPage tagsTab;
		private System.Windows.Forms.CheckedListBox senders;
		private System.Windows.Forms.TabPage sendersTab;
		private System.Windows.Forms.CheckedListBox tags;
		private System.Windows.Forms.Label logPanelHeaderLabel;
		private System.Windows.Forms.Panel logPanelHeader;
		private System.Windows.Forms.Label propertiesPanelHeaderLabel;
		private System.Windows.Forms.Panel propertiesPanelHeader;
		private System.Windows.Forms.CheckBox typeInformations;
		private System.Windows.Forms.CheckBox typeWarnings;
		private System.Windows.Forms.CheckBox typeErrors;
		private System.Windows.Forms.TabPage typesTab;
		private System.Windows.Forms.TabControl propertiesTabs;
		private System.Windows.Forms.Panel propertiesPanel;
		private System.Windows.Forms.ToolStripMenuItem closeMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem openMenu;
		private System.Windows.Forms.MenuStrip menu;
	}
}
