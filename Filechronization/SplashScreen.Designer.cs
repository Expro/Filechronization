/*
 *
 * User: Expro
 * Date: 2010-07-18
 * Time: 03:33
 * 
 * 
 */
namespace Filechronization
{
	partial class SplashScreen
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
			this.activityProgress = new System.Windows.Forms.ProgressBar();
			this.activity = new System.Windows.Forms.Label();
			this.image = new System.Windows.Forms.PictureBox();
			this.progressText = new System.Windows.Forms.Label();
			this.progress = new System.Windows.Forms.ProgressBar();
			this.activityCountLabel = new System.Windows.Forms.Label();
			this.progressCountLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.image)).BeginInit();
			this.SuspendLayout();
			// 
			// activityProgress
			// 
			this.activityProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.activityProgress.Location = new System.Drawing.Point(12, 173);
			this.activityProgress.Maximum = 3;
			this.activityProgress.Name = "activityProgress";
			this.activityProgress.Size = new System.Drawing.Size(499, 23);
			this.activityProgress.Step = 1;
			this.activityProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.activityProgress.TabIndex = 0;
			this.activityProgress.UseWaitCursor = true;
			// 
			// activity
			// 
			this.activity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.activity.Location = new System.Drawing.Point(12, 155);
			this.activity.Name = "activity";
			this.activity.Size = new System.Drawing.Size(405, 15);
			this.activity.TabIndex = 1;
			this.activity.Text = "Activity";
			this.activity.UseWaitCursor = true;
			// 
			// image
			// 
			this.image.Image = ((System.Drawing.Image)(resources.GetObject("image.Image")));
			this.image.Location = new System.Drawing.Point(12, 12);
			this.image.Name = "image";
			this.image.Size = new System.Drawing.Size(500, 140);
			this.image.TabIndex = 2;
			this.image.TabStop = false;
			this.image.UseWaitCursor = true;
			// 
			// progressText
			// 
			this.progressText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.progressText.Location = new System.Drawing.Point(12, 199);
			this.progressText.Name = "progressText";
			this.progressText.Size = new System.Drawing.Size(405, 14);
			this.progressText.TabIndex = 3;
			this.progressText.Text = "ProgressText";
			this.progressText.UseWaitCursor = true;
			// 
			// progress
			// 
			this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.progress.Location = new System.Drawing.Point(12, 216);
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size(499, 23);
			this.progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progress.TabIndex = 4;
			this.progress.UseWaitCursor = true;
			// 
			// activityCountLabel
			// 
			this.activityCountLabel.Location = new System.Drawing.Point(423, 157);
			this.activityCountLabel.Name = "activityCountLabel";
			this.activityCountLabel.Size = new System.Drawing.Size(88, 13);
			this.activityCountLabel.TabIndex = 5;
			this.activityCountLabel.Text = "0/0";
			this.activityCountLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// progressCountLabel
			// 
			this.progressCountLabel.Location = new System.Drawing.Point(423, 199);
			this.progressCountLabel.Name = "progressCountLabel";
			this.progressCountLabel.Size = new System.Drawing.Size(89, 16);
			this.progressCountLabel.TabIndex = 6;
			this.progressCountLabel.Text = "0/0";
			this.progressCountLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// SplashScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(523, 251);
			this.ControlBox = false;
			this.Controls.Add(this.progressCountLabel);
			this.Controls.Add(this.activityCountLabel);
			this.Controls.Add(this.progress);
			this.Controls.Add(this.progressText);
			this.Controls.Add(this.image);
			this.Controls.Add(this.activity);
			this.Controls.Add(this.activityProgress);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SplashScreen";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Filechronization Loading...";
			this.UseWaitCursor = true;
			this.Shown += new System.EventHandler(this.SplashScreenShown);
			((System.ComponentModel.ISupportInitialize)(this.image)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label progressCountLabel;
		private System.Windows.Forms.Label activityCountLabel;
		private System.Windows.Forms.ProgressBar activityProgress;
		private System.Windows.Forms.Label progressText;
		private System.Windows.Forms.PictureBox image;
		private System.Windows.Forms.Label activity;
		private System.Windows.Forms.ProgressBar progress;
	}
}
