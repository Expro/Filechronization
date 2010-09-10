using System.Windows.Forms;

namespace ExtendedControls
{
	partial class AcceptCancelButtons: UserControl
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
			this.acceptButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// acceptButton
			// 
			this.acceptButton.Dock = System.Windows.Forms.DockStyle.Left;
			this.acceptButton.Location = new System.Drawing.Point(0, 0);
			this.acceptButton.Name = "acceptButton";
			this.acceptButton.Size = new System.Drawing.Size(75, 25);
			this.acceptButton.TabIndex = 0;
			this.acceptButton.Text = "Accept";
			this.acceptButton.UseVisualStyleBackColor = true;
			this.acceptButton.Click += new System.EventHandler(this.AcceptButtonClick);
			// 
			// cancelButton
			// 
			this.cancelButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.cancelButton.Location = new System.Drawing.Point(90, 0);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 25);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// AcceptCancelButtons
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.acceptButton);
			this.Name = "AcceptCancelButtons";
			this.Size = new System.Drawing.Size(165, 25);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button acceptButton;
	}
}
