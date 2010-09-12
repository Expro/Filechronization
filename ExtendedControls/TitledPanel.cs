/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-12
 * Time: 17:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ExtendedControls
{
	/// <summary>
	/// Description of TitledPanel.
	/// </summary>
	public partial class TitledPanel: UserControl
	{
		public TitledPanel()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		
		public ControlCollection ContentControls
		{
			get {return contentPanel.Controls;}
		}
		
		public ControlCollection HeaderControls
		{
			get {return headerContentPanel.Controls;}
		}
		
		public Font TitleFont
		{
			get {return titleLabel.Font;}
			set {titleLabel.Font = value;}
		}
		
		public Color TitleColor
		{
			get {return titleLabel.ForeColor;}
			set {titleLabel.ForeColor = value;}
		}
		
		public string Title
		{
			get {return titleLabel.Text;}
			set {titleLabel.Text = value;}
		}
	}
}
