using System.Windows.Forms;
/*
 *
 * User: Expro
 * Date: 2010-07-31
 * Time: 16:51
 * 
 * 
 */
namespace FilechronizationGUI
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
			if (IsHandleCreated)
			{
				Invoke(new MethodInvoker(() =>
												{
													if (disposing)
													{
														if (components != null)
															components.Dispose();
													}
													
													base.Dispose(disposing);
				                         }));
			}
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.menu = new System.Windows.Forms.MenuStrip();
			this.applicationMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.minimizeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.applicationSeparator1Menu = new System.Windows.Forms.ToolStripSeparator();
			this.closeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.tabs = new System.Windows.Forms.TabControl();
			this.menu.SuspendLayout();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menu
			// 
			this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.applicationMenu});
			this.menu.Location = new System.Drawing.Point(0, 0);
			this.menu.Name = "menu";
			this.menu.Size = new System.Drawing.Size(809, 24);
			this.menu.TabIndex = 0;
			this.menu.Text = "menuStrip1";
			// 
			// applicationMenu
			// 
			this.applicationMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.minimizeMenu,
									this.applicationSeparator1Menu,
									this.closeMenu});
			this.applicationMenu.Name = "applicationMenu";
			this.applicationMenu.Size = new System.Drawing.Size(62, 20);
			this.applicationMenu.Text = "Application";
			// 
			// minimizeMenu
			// 
			this.minimizeMenu.Name = "minimizeMenu";
			this.minimizeMenu.Size = new System.Drawing.Size(112, 22);
			this.minimizeMenu.Text = "Minimize";
			// 
			// applicationSeparator1Menu
			// 
			this.applicationSeparator1Menu.Name = "applicationSeparator1Menu";
			this.applicationSeparator1Menu.Size = new System.Drawing.Size(109, 6);
			// 
			// closeMenu
			// 
			this.closeMenu.Name = "closeMenu";
			this.closeMenu.Size = new System.Drawing.Size(112, 22);
			this.closeMenu.Text = "Close";
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.tabs);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(809, 472);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(809, 497);
			this.toolStripContainer1.TabIndex = 1;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// tabs
			// 
			this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabs.Location = new System.Drawing.Point(0, 0);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(809, 472);
			this.tabs.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(809, 521);
			this.Controls.Add(this.toolStripContainer1);
			this.Controls.Add(this.menu);
			this.MainMenuStrip = this.menu;
			this.Name = "MainForm";
			this.Text = "FilechronizationGUI";
			this.menu.ResumeLayout(false);
			this.menu.PerformLayout();
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.ToolStripMenuItem closeMenu;
		private System.Windows.Forms.ToolStripSeparator applicationSeparator1Menu;
		private System.Windows.Forms.ToolStripMenuItem minimizeMenu;
		private System.Windows.Forms.ToolStripMenuItem applicationMenu;
		private System.Windows.Forms.MenuStrip menu;
	}
}
