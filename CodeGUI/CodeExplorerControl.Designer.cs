/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-12
 * Time: 16:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace CodeGUI
{
	partial class CodeExplorerControl
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
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.codeList = new System.Windows.Forms.TreeView();
			this.propertiesContainer = new System.Windows.Forms.ToolStripContainer();
			this.authorsLabel = new System.Windows.Forms.Label();
			this.version = new System.Windows.Forms.TextBox();
			this.versionLabel = new System.Windows.Forms.Label();
			this.name = new System.Windows.Forms.TextBox();
			this.nameLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.propertiesContainer.ContentPanel.SuspendLayout();
			this.propertiesContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.codeList);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.propertiesContainer);
			this.splitContainer.Size = new System.Drawing.Size(622, 520);
			this.splitContainer.SplitterDistance = 207;
			this.splitContainer.TabIndex = 0;
			// 
			// codeList
			// 
			this.codeList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.codeList.Location = new System.Drawing.Point(0, 0);
			this.codeList.Name = "codeList";
			this.codeList.Size = new System.Drawing.Size(207, 520);
			this.codeList.TabIndex = 0;
			// 
			// propertiesContainer
			// 
			// 
			// propertiesContainer.ContentPanel
			// 
			this.propertiesContainer.ContentPanel.Controls.Add(this.authorsLabel);
			this.propertiesContainer.ContentPanel.Controls.Add(this.version);
			this.propertiesContainer.ContentPanel.Controls.Add(this.versionLabel);
			this.propertiesContainer.ContentPanel.Controls.Add(this.name);
			this.propertiesContainer.ContentPanel.Controls.Add(this.nameLabel);
			this.propertiesContainer.ContentPanel.Size = new System.Drawing.Size(411, 495);
			this.propertiesContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertiesContainer.Location = new System.Drawing.Point(0, 0);
			this.propertiesContainer.Name = "propertiesContainer";
			this.propertiesContainer.Size = new System.Drawing.Size(411, 520);
			this.propertiesContainer.TabIndex = 0;
			this.propertiesContainer.Text = "toolStripContainer1";
			// 
			// authorsLabel
			// 
			this.authorsLabel.Location = new System.Drawing.Point(3, 76);
			this.authorsLabel.Name = "authorsLabel";
			this.authorsLabel.Size = new System.Drawing.Size(100, 23);
			this.authorsLabel.TabIndex = 4;
			this.authorsLabel.Text = "Authors:";
			// 
			// version
			// 
			this.version.Location = new System.Drawing.Point(73, 40);
			this.version.Name = "version";
			this.version.ReadOnly = true;
			this.version.Size = new System.Drawing.Size(335, 20);
			this.version.TabIndex = 3;
			// 
			// versionLabel
			// 
			this.versionLabel.Location = new System.Drawing.Point(3, 43);
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.Size = new System.Drawing.Size(64, 23);
			this.versionLabel.TabIndex = 2;
			this.versionLabel.Text = "Version:";
			// 
			// name
			// 
			this.name.Location = new System.Drawing.Point(73, 7);
			this.name.Name = "name";
			this.name.ReadOnly = true;
			this.name.Size = new System.Drawing.Size(335, 20);
			this.name.TabIndex = 1;
			// 
			// nameLabel
			// 
			this.nameLabel.Location = new System.Drawing.Point(3, 10);
			this.nameLabel.Name = "nameLabel";
			this.nameLabel.Size = new System.Drawing.Size(64, 23);
			this.nameLabel.TabIndex = 0;
			this.nameLabel.Text = "Name:";
			// 
			// CodeExplorerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer);
			this.Name = "CodeExplorerControl";
			this.Size = new System.Drawing.Size(622, 520);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.propertiesContainer.ContentPanel.ResumeLayout(false);
			this.propertiesContainer.ContentPanel.PerformLayout();
			this.propertiesContainer.ResumeLayout(false);
			this.propertiesContainer.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label nameLabel;
		private System.Windows.Forms.TextBox name;
		private System.Windows.Forms.Label versionLabel;
		private System.Windows.Forms.TextBox version;
		private System.Windows.Forms.Label authorsLabel;
		private System.Windows.Forms.ToolStripContainer propertiesContainer;
		private System.Windows.Forms.TreeView codeList;
		private System.Windows.Forms.SplitContainer splitContainer;
	}
}
