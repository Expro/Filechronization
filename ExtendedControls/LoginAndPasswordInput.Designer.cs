using System.Windows.Forms;
/*
 *
 * User: Expro
 * Date: 2010-06-26
 * Time: 18:48
 * 
 */
namespace ExtendedControls
{
	partial class LoginAndPasswordInput
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
			this.controlLayout = new System.Windows.Forms.TableLayoutPanel();
			this.passwordLabel = new System.Windows.Forms.Label();
			this.loginLabel = new System.Windows.Forms.Label();
			this.login = new ExtendedControls.ExTextBox();
			this.password = new ExtendedControls.ExTextBox();
			this.controlLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// controlLayout
			// 
			this.controlLayout.ColumnCount = 2;
			this.controlLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.controlLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
			this.controlLayout.Controls.Add(this.passwordLabel, 0, 1);
			this.controlLayout.Controls.Add(this.loginLabel, 0, 0);
			this.controlLayout.Controls.Add(this.login, 1, 0);
			this.controlLayout.Controls.Add(this.password, 1, 1);
			this.controlLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controlLayout.Location = new System.Drawing.Point(0, 0);
			this.controlLayout.MinimumSize = new System.Drawing.Size(270, 60);
			this.controlLayout.Name = "controlLayout";
			this.controlLayout.RowCount = 2;
			this.controlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.controlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.controlLayout.Size = new System.Drawing.Size(270, 60);
			this.controlLayout.TabIndex = 0;
			// 
			// passwordLabel
			// 
			this.passwordLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.passwordLabel.Location = new System.Drawing.Point(3, 30);
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.Size = new System.Drawing.Size(61, 30);
			this.passwordLabel.TabIndex = 2;
			this.passwordLabel.Text = "Password:";
			this.passwordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// loginLabel
			// 
			this.loginLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.loginLabel.Location = new System.Drawing.Point(3, 0);
			this.loginLabel.Name = "loginLabel";
			this.loginLabel.Size = new System.Drawing.Size(61, 30);
			this.loginLabel.TabIndex = 0;
			this.loginLabel.Text = "Login:";
			this.loginLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// login
			// 
			this.login.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.login.BackColorActive = System.Drawing.Color.LightYellow;
			this.login.Description = "at least 6 characters, up to 12 characters";
			this.login.DescriptionColor = System.Drawing.SystemColors.GrayText;
			this.login.EnterAsNext = true;
			this.login.ForeColor = System.Drawing.SystemColors.GrayText;
			this.login.Location = new System.Drawing.Point(70, 5);
			this.login.Margin = new System.Windows.Forms.Padding(3, 3, 25, 3);
			this.login.MaxLength = 12;
			this.login.Name = "login";
			this.login.Size = new System.Drawing.Size(175, 20);
			this.login.TabIndex = 1;
			this.login.Text = "at least 6 characters, up to 12 characters";
			this.login.TextColor = System.Drawing.SystemColors.WindowText;
			this.login.Validating += new System.ComponentModel.CancelEventHandler(this.LoginValidating);
			this.login.EnterPress += new System.EventHandler(this.LoginEnterPress);
			// 
			// password
			// 
			this.password.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.password.BackColorActive = System.Drawing.Color.LightYellow;
			this.password.Description = "at least 8 characters, up to 12 characters";
			this.password.DescriptionColor = System.Drawing.SystemColors.GrayText;
			this.password.EnterAsNext = true;
			this.password.ForeColor = System.Drawing.SystemColors.GrayText;
			this.password.Location = new System.Drawing.Point(70, 35);
			this.password.Margin = new System.Windows.Forms.Padding(3, 3, 25, 3);
			this.password.MaxLength = 12;
			this.password.Name = "password";
			this.password.PasswordChar = '*';
			this.password.Size = new System.Drawing.Size(175, 20);
			this.password.TabIndex = 3;
			this.password.Text = "at least 8 characters, up to 12 characters";
			this.password.TextColor = System.Drawing.SystemColors.WindowText;
			this.password.Validating += new System.ComponentModel.CancelEventHandler(this.PasswordValidating);
			this.password.EnterPress += new System.EventHandler(this.PasswordEnterPress);
			// 
			// LoginAndPasswordInput
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.controlLayout);
			this.MinimumSize = new System.Drawing.Size(270, 60);
			this.Name = "LoginAndPasswordInput";
			this.Size = new System.Drawing.Size(270, 60);
			this.controlLayout.ResumeLayout(false);
			this.controlLayout.PerformLayout();
			this.ResumeLayout(false);
		}
		private ExtendedControls.ExTextBox password;
		private ExtendedControls.ExTextBox login;
		private System.Windows.Forms.Label loginLabel;
		private System.Windows.Forms.Label passwordLabel;
		private System.Windows.Forms.TableLayoutPanel controlLayout;
		
		private void LoginEnterPress(object sender, System.EventArgs e)
		{
			password.Focus();
		}
		
		private void PasswordEnterPress(object sender, System.EventArgs e)
		{
			OnEnterPress(new System.EventArgs());
		}
	}
}
