/*
 *
 * User: Expro
 * Date: 2010-07-31
 * Time: 16:51
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeManagement;
using CodeManagement.Definitions;

namespace FilechronizationGUI
{
	[Name("Filechronization GUI")]
	[Version(1, 0, 0)]
	[Author("Maciej 'Expro' Grabowski", "mds.expro@gmail.com")]
	[Description("Form containing all graphical subelements required to provide feedback for user.")]
	[Module("GUI")]
	public partial class MainForm: Form, ICode, IControllableCode, IFilechronizationMainWindow
	{
		public MainForm()
		{
			Application.EnableVisualStyles();
			InitializeComponent();
			CreateControl();
			CreateHandle();
		}

		public Control AddContentToCenter(string title, Control content)
		{
			TabPage tab;

			if (content == null)
				throw new ArgumentNullException("content");
			if (title == null)
				throw new ArgumentNullException("name");

			tab = new TabPage(title);
			tab.Controls.Add(content);
			content.Dock = DockStyle.Fill;

			tabs.TabPages.Add(tab);

			return tab;
		}

		public void Start()
		{
			Application.Run(this);
		}

		public void Pause()
		{
			Invoke(new MethodInvoker(() => {Enabled = false;}));
		}

		public void Restore()
		{
			Invoke(new MethodInvoker(() => {Enabled = true;}));
		}

		public void End()
		{
			Application.ExitThread();
		}
	}
}
