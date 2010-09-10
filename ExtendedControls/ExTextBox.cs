/*
 *
 * User: Expro
 * Date: 2010-06-26
 * Time: 13:33
 * 
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace ExtendedControls
{
	#region Comment
	/// <summary>
	/// 	Description of ExTextBox.
	/// </summary>
	#endregion
	[DefaultPropertyAttribute("Name")]
	public class ExTextBox: TextBox
	{
		private Color textColor;
		private Color descriptionColor;
		private Color backColorActive;
		private Color backColor;
		private string description;
		private string text;
		private char passwordChar;
		private bool enterAsNext;
		
		private void DisplayDescription()
		{
			base.PasswordChar = '\0';
			ForeColor = descriptionColor;
			base.Text = description;
		}
		
		private void DisplayText()
		{
			base.PasswordChar = passwordChar;
			ForeColor = textColor;
			base.Text = text;
		}
		
		protected override void OnTextChanged(EventArgs e)
		{
			if ((base.Text != description) && (!text.Equals(base.Text)))
			{
				text = base.Text;
				base.OnTextChanged(e);
			}
		}
		
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if ((e.KeyChar == 13) && (EnterAsNext))
			{
				e.Handled = true;
				OnEnterPress(new EventArgs());
			}
			
			base.OnKeyPress(e);
		}
		
		protected virtual void OnEnterPress(EventArgs e)
		{
			if (EnterPress != null)
				EnterPress(this, e);
		}
		
		private void ExGotFocus(object sender, EventArgs e)
		{
			base.BackColor = backColorActive;
			DisplayText();
		}
		
		private void ExLostFocus(object sender, EventArgs e)
		{
			base.BackColor = BackColor;
			if (IsDescriptionRequired)
				DisplayDescription();
		}
		
		private bool IsDescriptionRequired
		{
			get
			{
				return (!Focused) && (String.IsNullOrEmpty(text));
			}
		}
		
		public ExTextBox()
		{
			descriptionColor = SystemColors.GrayText;
			textColor = SystemColors.WindowText;
			backColor = SystemColors.Window;
			backColorActive = SystemColors.Info;
			
			text = "";
			description = "Text field description.";
			enterAsNext = false;
			
			GotFocus += ExGotFocus;
			LostFocus += ExLostFocus;
		}
		
		public new Color BackColor
		{
			get {return backColor;}
			
			set
			{
				backColor = value;
				
				if (!Focused)
					base.BackColor = backColor;
			}
		}
		
		[CategoryAttribute("Appearance")]
		[DescriptionAttribute("Backcolor of focused ExTextBox.")]
		[BrowsableAttribute(true)]
		public Color BackColorActive
		{
			get {return backColorActive;}
			
			set
			{
				backColorActive = value;
				
				if (Focused)
					base.BackColor = backColorActive;
			}
		}
		
		[CategoryAttribute("Appearance")]
		[DescriptionAttribute("Color of description displayed on empty and not focused ExTextBox.")]
		[BrowsableAttribute(true)]
		public Color DescriptionColor
		{
			get {return descriptionColor;}
			
			set
			{
				descriptionColor = value;
				
				if (IsDescriptionRequired)
					DisplayDescription();
			}
		}
		
		[CategoryAttribute("Appearance")]
		[DescriptionAttribute("Color of text contained by ExTextBox.")]
		[BrowsableAttribute(true)]
		public Color TextColor
		{
			get {return textColor;}
			
			set
			{
				textColor = value;
				
				if (!IsDescriptionRequired)
					DisplayText();
			}
		}
		
		[CategoryAttribute("Appearance")]
		[DescriptionAttribute("Text displayed when ExTextBox contains empty string and is not focused.")]
		[BrowsableAttribute(true)]
		public string Description
		{
			get {return description;}
			
			set
			{
				description = value;
				
				if (IsDescriptionRequired)
					DisplayDescription();
			}
		}
		
		public new string Text
		{
			get {return text;}
			
			set
			{
				if (value != null)
					base.Text = value;
				else
					base.Text = "";
				
				if (IsDescriptionRequired)
					DisplayDescription();
				else
					DisplayText();
			}
		}

		public new char PasswordChar
		{
			get
			{
				return passwordChar;
			}
			
			set
			{
				passwordChar = value;
				
				if (!IsDescriptionRequired)
					DisplayText();
			}
		}
		
		[CategoryAttribute("Behavior")]
		[DescriptionAttribute("Determines if Enter (Return) character should be ignored. If set to true, EnterDown evemt will be rised on each Enter character.")]
		[BrowsableAttribute(true)]
		public bool EnterAsNext
		{
			get {return enterAsNext;}
			set {enterAsNext = value;}
		}
		
		[CategoryAttribute("Key")]
		[DescriptionAttribute("Provides informations about Enter (Return) characters pressed while EnterAsNext is set to true.")]
		[BrowsableAttribute(true)]
		public event EventHandler EnterPress;
	}
}
