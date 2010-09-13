/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-12
 * Time: 16:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CodeManagement.Definitions;
using FilechronizationGUI;

namespace CodeGUI
{
	[Name("Code Management GUI")]
	[Version(1, 0, 0)]
	[Author("Expro", "mds.expro@gmail.com")]
	[CodeManagement.Definitions.Description("Provides control over code usable with Filechronization")]
	//[Module("GUI")]
	public partial class CodeExplorerControl: UserControl, ICode
	{
		private Control windowControl;
		
		public CodeExplorerControl(IFilechronizationMainWindow mainWindow)
		{
			if (mainWindow == null)
				throw new ArgumentNullException("mainWindow");
			
			InitializeComponent();
			
			windowControl = mainWindow.AddContentToCenter("Code", this);
		}
	}
}
