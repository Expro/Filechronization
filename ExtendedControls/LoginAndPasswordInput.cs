/*
 *
 * User: Expro
 * Date: 2010-06-26
 * Time: 18:48
 * 
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ExtendedControls
{
	public class IsCorrectChangedEventArgs: EventArgs
	{
		private bool isCorrect;
		
		public IsCorrectChangedEventArgs(bool isCorrect)
		{
			this.isCorrect = isCorrect;
		}
		
		public bool IsCorrect
		{
			get {return isCorrect;}
		}
	}
	
	#region Comment
	/// <summary>
	/// 	Description of LoginFrame.
	/// </summary>
	#endregion
	public partial class LoginAndPasswordInput: UserControl
	{
		private ErrorProvider errorProvider;
		private int loginMinLength;
		private int loginMaxLength;
		private int passwordMinLength;
		private int passwordMaxLength;
		
		private string lastCorrectLogin;
		private bool isLoginCorrect;
		private string lastCorrectPassword;
		private bool isPasswordCorrect;
		private bool isCorrect;
		
		private void LoginRefreshDescription()
		{
			login.Description = String.Format("at least {0} characters, up to {1} characters", loginMinLength, loginMaxLength);
		}
		
		private void PasswordRefreshDescription()
		{
			password.Description = String.Format("at least {0} characters, up to {1} characters", passwordMinLength, passwordMaxLength);
		}
		
		private void LoginValidating(object sender, CancelEventArgs e)
		{
			if (login.Text.Length < loginMinLength)
			{
				IsLoginCorrect = false;
				errorProvider.SetError(login, String.Format("Current input is too short. At least {0} characters are required.", loginMinLength));
			}
			else
			{
				IsLoginCorrect = true;
				lastCorrectLogin = login.Text;
				errorProvider.SetError(login, "");
			}
		}
		
		private void PasswordValidating(object sender, CancelEventArgs e)
		{
			if (password.Text.Length < passwordMinLength)
			{
				IsPasswordCorrect = false;
				errorProvider.SetError(password, String.Format("Current input is too short. At least {0} characters are required.", passwordMinLength));
			}
			else
			{
				IsPasswordCorrect = true;
				lastCorrectPassword = password.Text;
				errorProvider.SetError(password, "");
			}
		}
		
		private void SetIsCorrect(bool value)
		{
			bool fireEvent = (value != isCorrect);
			
			isCorrect = value;
			
			if (fireEvent)
				OnIsCorrectChanged(new IsCorrectChangedEventArgs(isCorrect));
		}
		
		private bool IsLoginCorrect
		{
			get {return isLoginCorrect;}
			
			set
			{
				isLoginCorrect = value;
				
				SetIsCorrect(isLoginCorrect && isPasswordCorrect);
			}
		}
		
		private bool IsPasswordCorrect
		{
			get {return isPasswordCorrect;}
			
			set
			{
				isPasswordCorrect = value;
				
				SetIsCorrect(isLoginCorrect && isPasswordCorrect);
			}
		}
		
		protected virtual void OnIsCorrectChanged(IsCorrectChangedEventArgs e)
		{
			if (IsCorrectChanged != null)
				IsCorrectChanged(this, e);
		}
		
		protected virtual void OnEnterPress(EventArgs e)
		{
			if (EnterPress != null)
				EnterPress(this, e);
				
		}
		
		public LoginAndPasswordInput()
		{
			InitializeComponent();
			
			errorProvider = new ErrorProvider(this);
			errorProvider.BlinkRate = 0;
			
			loginMinLength = 6;
			loginMaxLength = 12;
			
			passwordMinLength = 8;
			passwordMaxLength = 12;
			
			lastCorrectLogin = "";
			lastCorrectPassword = "";
			
			isLoginCorrect = false;
			isPasswordCorrect = false;
			isCorrect = false;
		}
		
		public void Clear()
		{
			login.Text = "";
			password.Text = "";
			
			errorProvider.SetError(login, "");
			errorProvider.SetError(password, "");
			
			IsLoginCorrect = false;
			IsPasswordCorrect = false;
			
			SetIsCorrect(false);
		}
		
		[CategoryAttribute("Security")]
		[DescriptionAttribute("Minimal count of characters required in login.")]
		[BrowsableAttribute(true)]
		public int LoginMinLength
		{
			get {return loginMinLength;}
			
			set
			{
				if ((value > 0) && (value < loginMaxLength))
				{
					loginMinLength = value;
					
					LoginRefreshDescription();
				}
			}
		}
		
		[CategoryAttribute("Security")]
		[DescriptionAttribute("Maximal count of characters required in login.")]
		[BrowsableAttribute(true)]
		public int LoginMaxLength
		{
			get {return loginMaxLength;}
			
			set
			{
				if ((value > 0) && (value > loginMinLength))
				{
					loginMaxLength = value;
					login.MaxLength = loginMaxLength;
					
					LoginRefreshDescription();
				}
			}
		}
		
		[CategoryAttribute("Security")]
		[DescriptionAttribute("Minimal count of characters required in password.")]
		[BrowsableAttribute(true)]
		public int PasswordMinLength
		{
			get {return passwordMinLength;}
			
			set
			{
				if ((value > 0) && (value < passwordMaxLength))
				{
					passwordMinLength = value;
					
					PasswordRefreshDescription();
				}
			}
		}
		
		[CategoryAttribute("Security")]
		[DescriptionAttribute("Maximal count of characters required in password.")]
		[BrowsableAttribute(true)]
		public int PasswordMaxLength
		{
			get {return passwordMaxLength;}
			
			set
			{
				if ((value > 0) && (value > passwordMinLength))
				{
					passwordMaxLength = value;
					password.MaxLength = passwordMaxLength;
					
					PasswordRefreshDescription();
				}
			}
		}
		
		public string LastCorrectPassword
		{
			get {return lastCorrectPassword;}
		}
		
		public string LastCorrectLogin
		{
			get {return lastCorrectLogin;}
		}
		
		public bool IsCorrect
		{
			get {return isCorrect;}
		}
		
		[CategoryAttribute("Property Changed")]
		[DescriptionAttribute("Provides information about state changes between correct data and incorrect data.")]
		[BrowsableAttribute(true)]
		public event EventHandler<IsCorrectChangedEventArgs> IsCorrectChanged;
		
		[CategoryAttribute("Key")]
		[DescriptionAttribute("Provides information about Enter (Return) character pressed in Password field.")]
		[BrowsableAttribute(true)]
		public event EventHandler EnterPress;
	}
}
