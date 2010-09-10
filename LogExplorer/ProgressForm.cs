/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-03
 * Time: 02:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using Patterns;

namespace LogExplorer
{
	public partial class ProgressForm: Form
	{
		private DateTime lastRefresh;
		private DateTime begin;
		
		public ProgressForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			CreateHandle();
			CreateControl();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			this.lastRefresh = DateTime.Now;
		}
	
		public void ProgressHadler(object sender, ProgressEventArgs e)
		{
			if (!Visible)
				Show();
			
			if (e.Step == 0)
			{
				progressBar.Maximum = e.Steps;
				begin = DateTime.Now;
			}
			progressBar.Value = e.Step;
			
			stepLabel.Text = e.Step.ToString() + "/" + e.Steps.ToString();
			actionNameLabel.Text = e.Process + " (" + e.ProcessedItem + ")";
			Text = "Loading... (" + DateTime.Now.Subtract(begin).ToString() + ")";
			
			if (DateTime.Now.Subtract(lastRefresh).Milliseconds > 200)
			{
				lastRefresh = DateTime.Now;
				Refresh();
			}
				
			if (e.Step == (e.Steps - 1))
				Hide();
		}
	}
}
