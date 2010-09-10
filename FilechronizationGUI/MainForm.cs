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
	public partial class MainForm: Form, IControllableCode
	{
		public MainForm()
		{
			Application.EnableVisualStyles();
			InitializeComponent();
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
			Invoke(new MethodInvoker(() => {Close();}));
		}
	}
}
