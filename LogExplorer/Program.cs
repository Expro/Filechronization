/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-31
 * Time: 15:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace LogExplorer
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			MainWindowViewModel viewModel = new MainWindowViewModel();
			Model model = new Model(viewModel);
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm(viewModel));
		}
		
	}
}
