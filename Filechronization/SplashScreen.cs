/*
 *
 * User: Expro
 * Date: 2010-07-18
 * Time: 03:33
 * 
 * 
 */
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using CodeManagement;
using Patterns;

namespace Filechronization
{
	/// <summary>
	/// Description of SplashScreen.
	/// </summary>
	public partial class SplashScreen : Form
	{
		private Semaphore splashLock;
		
		public SplashScreen(Semaphore splashLock)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			this.splashLock = splashLock;
		}
		
		public string Activity
		{
			get {return activity.Text;}
			
			set
			{
				activity.Invoke(new MethodInvoker(delegate() {activity.Text = value;}));
			}
		}
		
		void SplashScreenShown(object sender, EventArgs e)
		{
			splashLock.Release();
		}
		
		public void ProgressHandler(object sender, ProgressEventArgs e)
		{
			progress.Invoke(new MethodInvoker(delegate()
														{
				                                  	if (!activity.Text.Equals(e.Process))
				                                  		activityProgress.PerformStep();
				                                  	activity.Text = e.Process;
				                                  	activityCountLabel.Text = activityProgress.Value.ToString() + "/3";
			                                  	
				                                  	progressText.Text = e.ProcessedItem;
				                                  	progressCountLabel.Text = e.Step.ToString() + "/" + e.Steps;
															
															if (e.Step == 0)
																progress.Maximum = e.Steps;
															
															progress.Value = e.Step;
			                                  }));
		}
	}
}
