/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-12
 * Time: 17:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ExtendedControls
{
	partial class TitledPanel
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
			this.headerPanel = new System.Windows.Forms.Panel();
			this.headerContentPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.titleLabel = new System.Windows.Forms.Label();
			this.contentPanel = new System.Windows.Forms.Panel();
			this.headerPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// headerPanel
			// 
			this.headerPanel.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.headerPanel.Controls.Add(this.headerContentPanel);
			this.headerPanel.Controls.Add(this.titleLabel);
			this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.headerPanel.Location = new System.Drawing.Point(0, 0);
			this.headerPanel.Name = "headerPanel";
			this.headerPanel.Size = new System.Drawing.Size(400, 21);
			this.headerPanel.TabIndex = 0;
			// 
			// headerContentPanel
			// 
			this.headerContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.headerContentPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.headerContentPanel.Location = new System.Drawing.Point(33, 0);
			this.headerContentPanel.Name = "headerContentPanel";
			this.headerContentPanel.Size = new System.Drawing.Size(367, 21);
			this.headerContentPanel.TabIndex = 1;
			// 
			// titleLabel
			// 
			this.titleLabel.AutoSize = true;
			this.titleLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.titleLabel.Location = new System.Drawing.Point(0, 0);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Padding = new System.Windows.Forms.Padding(3);
			this.titleLabel.Size = new System.Drawing.Size(33, 19);
			this.titleLabel.TabIndex = 0;
			this.titleLabel.Text = "Title";
			this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// contentPanel
			// 
			this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentPanel.Location = new System.Drawing.Point(0, 21);
			this.contentPanel.Name = "contentPanel";
			this.contentPanel.Size = new System.Drawing.Size(400, 227);
			this.contentPanel.TabIndex = 1;
			// 
			// TitledPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.contentPanel);
			this.Controls.Add(this.headerPanel);
			this.Name = "TitledPanel";
			this.Size = new System.Drawing.Size(400, 248);
			this.headerPanel.ResumeLayout(false);
			this.headerPanel.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.FlowLayoutPanel headerContentPanel;
		private System.Windows.Forms.Panel contentPanel;
		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.Panel headerPanel;
	}
}
