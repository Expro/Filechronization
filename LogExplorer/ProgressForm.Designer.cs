/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-03
 * Time: 02:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace LogExplorer
{
	partial class ProgressForm
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
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.stepLabel = new System.Windows.Forms.Label();
			this.actionNameLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(12, 25);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(479, 23);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar.TabIndex = 0;
			// 
			// stepLabel
			// 
			this.stepLabel.Location = new System.Drawing.Point(391, 9);
			this.stepLabel.Name = "stepLabel";
			this.stepLabel.Size = new System.Drawing.Size(100, 13);
			this.stepLabel.TabIndex = 1;
			this.stepLabel.Text = "0/0";
			this.stepLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// actionNameLabel
			// 
			this.actionNameLabel.Location = new System.Drawing.Point(12, 9);
			this.actionNameLabel.Name = "actionNameLabel";
			this.actionNameLabel.Size = new System.Drawing.Size(361, 13);
			this.actionNameLabel.TabIndex = 2;
			this.actionNameLabel.Text = "Loading log entries...";
			this.actionNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ProgressForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(503, 60);
			this.ControlBox = false;
			this.Controls.Add(this.actionNameLabel);
			this.Controls.Add(this.stepLabel);
			this.Controls.Add(this.progressBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Loading...";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label actionNameLabel;
		private System.Windows.Forms.Label stepLabel;
		private System.Windows.Forms.ProgressBar progressBar;
	}
}
