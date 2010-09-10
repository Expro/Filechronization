/*
 *
 * User: Expro
 * Date: 2010-06-26
 * Time: 23:54
 * 
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ExtendedControls
{
	public class LogInEventArgs: EventArgs
	{
		private string login;
		private string password;
		
		public LogInEventArgs(string login, string password)
		{
			this.login = login;
			this.password = password;
		}
		
		public string Login
		{
			get {return login;}
		}
		
		public string Password
		{
			get {return password;}
		}
	}
	
	#region Comment
	/// <summary>
	/// Description of LoginPanel.
	/// </summary>
	#endregion
	public partial class LoginPanel: UserControl
	{
		private void CancelButtonClick(object sender, EventArgs e)
		{
			loginData.Clear();
			OnCancel(new EventArgs());
		}
		
		private void LoginDataEnterPress(object sender, EventArgs e)
		{
			if (loginData.Validate())
				logInButton.Focus();
			else
				Focus();
		}
		
		private void LogInButtonClick(object sender, EventArgs e)
		{
			OnLogIn(new LogInEventArgs(loginData.LastCorrectLogin, loginData.LastCorrectPassword));
		}
		
		protected virtual void OnLogIn(LogInEventArgs e)
		{
			if (LogIn != null)
				LogIn(this, e);
		}
		
		protected virtual void OnCancel(EventArgs e)
		{
			if (Cancel != null)
				Cancel(this, e);
		}
		
		public LoginPanel()
		{
			InitializeComponent();
		}
		
		private void LogInDataIsCorrectChanged(object sender, IsCorrectChangedEventArgs e)
		{
			logInButton.Enabled = e.IsCorrect;
		}
		
		[CategoryAttribute("Security")]
		[DescriptionAttribute("Minimal count of characters required in login.")]
		[BrowsableAttribute(true)]
		public int LoginMinLength
		{
			get {return loginData.LoginMinLength;}			
			set {loginData.LoginMinLength = value;}
		}
		
		[CategoryAttribute("Security")]
		[DescriptionAttribute("Maximal count of characters required in login.")]
		[BrowsableAttribute(true)]
		public int LoginMaxLength
		{
			get {return loginData.LoginMaxLength;}			
			set {loginData.LoginMaxLength = value;}
		}
		
		[CategoryAttribute("Security")]
		[DescriptionAttribute("Minimal count of characters required in password.")]
		[BrowsableAttribute(true)]
		public int PasswordMinLength
		{
			get {return loginData.PasswordMinLength;}			
			set {loginData.PasswordMinLength = value;}
		}
		
		[CategoryAttribute("Security")]
		[DescriptionAttribute("Maximal count of characters required in password.")]
		[BrowsableAttribute(true)]
		public int PasswordMaxLength
		{
			get {return loginData.PasswordMaxLength;}			
			set {loginData.PasswordMaxLength = value;}
		}
		
		[CategoryAttribute("Behavior")]
		[DescriptionAttribute("Performent whenever Cancel button was pressed.")]
		[BrowsableAttribute(true)]
		public event EventHandler Cancel;
		
		[CategoryAttribute("Behavior")]
		[DescriptionAttribute("Performent whenever Cancel button was pressed.")]
		[BrowsableAttribute(true)]
		public event EventHandler<LogInEventArgs> LogIn;
	}
}
