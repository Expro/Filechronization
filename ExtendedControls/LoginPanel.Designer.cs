/*
 *
 * User: Expro
 * Date: 2010-06-26
 * Time: 23:54
 * 
 */
namespace ExtendedControls
{
	partial class LoginPanel
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
			this.loginData = new ExtendedControls.LoginAndPasswordInput();
			this.cancelButton = new System.Windows.Forms.Button();
			this.logInButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// loginData
			// 
			this.loginData.Dock = System.Windows.Forms.DockStyle.Top;
			this.loginData.Location = new System.Drawing.Point(0, 0);
			this.loginData.LoginMaxLength = 12;
			this.loginData.LoginMinLength = 6;
			this.loginData.MinimumSize = new System.Drawing.Size(250, 50);
			this.loginData.Name = "loginData";
			this.loginData.PasswordMaxLength = 12;
			this.loginData.PasswordMinLength = 8;
			this.loginData.Size = new System.Drawing.Size(270, 62);
			this.loginData.TabIndex = 0;
			this.loginData.IsCorrectChanged += new System.EventHandler<ExtendedControls.IsCorrectChangedEventArgs>(this.LoginDataIsCorrectChanged);
			this.loginData.EnterPress += new System.EventHandler(this.LoginDataEnterPress);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.Location = new System.Drawing.Point(192, 64);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// logInButton
			// 
			this.logInButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.logInButton.Enabled = false;
			this.logInButton.Location = new System.Drawing.Point(111, 64);
			this.logInButton.Name = "logInButton";
			this.logInButton.Size = new System.Drawing.Size(75, 23);
			this.logInButton.TabIndex = 1;
			this.logInButton.Text = "Log In";
			this.logInButton.UseVisualStyleBackColor = true;
			this.logInButton.Click += new System.EventHandler(this.LogInButtonClick);
			// 
			// LoginPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.logInButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.loginData);
			this.MinimumSize = new System.Drawing.Size(270, 90);
			this.Name = "LoginPanel";
			this.Size = new System.Drawing.Size(270, 90);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button logInButton;
		private System.Windows.Forms.Button cancelButton;
		private ExtendedControls.LoginAndPasswordInput loginData;
		
		void LoginDataIsCorrectChanged(object sender, IsCorrectChangedEventArgs e)
		{
			logInButton.Enabled = e.IsCorrect;
		}
	}
}
