
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ExtendedControls
{
	#region Comment
	/// <summary>
	/// 	Description of AcceptCancelButtons.
	/// </summary>
	#endregion
	public partial class AcceptCancelButtons: UserControl
	{
		private void AcceptButtonClick(object sender, EventArgs e)
		{
			OnAcceptClicked();
		}
		
		private void CancelButtonClick(object sender, EventArgs e)
		{
			OnCancelClicked();
		}
		
		protected void OnAcceptClicked()
		{
			if (AcceptClicked != null)
				AcceptClicked(this, new EventArgs());
		}
		
		protected void OnCancelClicked()
		{
			if (CancelClicked != null)
				CancelClicked(this, new EventArgs());
		}
		
		public AcceptCancelButtons()
		{
			InitializeComponent();
		}
		
		public bool AcceptEnabled
		{
			get {return acceptButton.Enabled;}
			set {acceptButton.Enabled = value;}
		}
		
		public bool CancelEnabled
		{
			get {return cancelButton.Enabled;}
			set {cancelButton.Enabled = value;}
		}
		
		public event EventHandler AcceptClicked;
		public event EventHandler CancelClicked;
	}
}
