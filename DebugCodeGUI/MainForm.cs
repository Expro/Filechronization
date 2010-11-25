/*
 *
 * User: Expro
 * Date: 2010-07-18
 * Time: 01:31
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeManagement;
using CodeManagement.Definitions;

namespace DebugCodeGUI
{
	[Name("Code Control Form")]
	[Version(1, 0, 0)]
	[Author("Maciej 'Expro' Grabowski", "mds.expro@gmail.com")]
	[Description("Simple modules, addins and components viewer and controller.")]
	[Description("For debug time only, at release version there will be replecament.")]
	[Module("DEBUG")]
	public partial class mainForm: Form, IContainerControl, IControllableCode
	{
		private ICodeController controller;
		
		public mainForm(CodeManager manager)
		{
			this.manager = manager;
			
			InitializeComponent();
			
			Application.EnableVisualStyles();
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		private CodeManager manager;
		
		public CodeManager Manager
		{
			get {return manager;}
		}
		
		void TypeSelectedIndexChanged(object sender, EventArgs e)
		{
			IEnumerable<CodeDetails> items = new List<CodeDetails>();
			
			list.Items.Clear();
			list.BeginUpdate();
			
			if (type.SelectedIndex == 0)
				items = manager.Modules;
			
			if (type.SelectedIndex == 1)
				items = manager.Addins;
			
			if (type.SelectedIndex == 2)
				items = manager.Shared;

			foreach (CodeDetails details in items)
			{
				if (details == null)
					throw new ArgumentNullException();
				
				list.Items.Add(details);
			}
			
			list.EndUpdate();
		}
		
		public void Start()
		{
			Application.Run(this);
		}
		
		public void Pause()
		{
			Invoke(new MethodInvoker(delegate () {Enabled = false;}));
		}
		
		public void Restore()
		{
			Invoke(new MethodInvoker(delegate () {Enabled = true;}));
		}
		
		public void End()
		{
			Invoke(new MethodInvoker(delegate () {Close();}));
		}
		
		void MainFormFormClosed(object sender, FormClosedEventArgs e)
		{
			manager.StopAndDestroyModules();
		}
		
		private void RefreshState()
		{
			if (controller != null)
			{
				unloadButton.Enabled = false;
				createButton.Enabled =	false;
				destroyButton.Enabled = false;
				LoadButton.Enabled = false;
				runButton.Enabled = false;
				startButton.Enabled = false;
				pauseButton.Enabled = false;
				restoreButton.Enabled = false;
				endButton.Enabled = false;
				initializeButton.Enabled = false;
				
				if (!controller.State.Equals(CodeState.Corrupted))
				{
					initializeButton.Enabled = true;
					
					switch (controller.State)
					{
						case CodeState.Unloaded:
							LoadButton.Enabled = true;
							break;
						case CodeState.Loading:
							break;
						case CodeState.Loaded:
							createButton.Enabled = true;
							unloadButton.Enabled = true;
							break;
						case CodeState.Creating:
							break;
						case CodeState.Created:
							destroyButton.Enabled = true;
							initializeButton.Enabled = false;
							break;
						case CodeState.Destroying:
							break;
						case CodeState.Corrupted:	
							break;
						default:
							throw new Exception("Invalid value for CodeState");
					}
					
					var cController = controller as ControllableCodeController;
					if (cController != null)
					{
						if (cController.State.Equals(CodeState.Created))
						{
							runButton.Enabled = !cController.StateOfControllable.Equals(ControllableState.Started);
							
							switch (cController.StateOfControllable)
							{
								case ControllableState.Started:
									pauseButton.Enabled = true;
									endButton.Enabled = true;
									break;
								case ControllableState.Paused:
									restoreButton.Enabled = true;
									break;
								case ControllableState.Stopped:
									startButton.Enabled = true;
									break;
								default:
									throw new Exception("Invalid value for ControllableState");
							}
								
						}
					}
				}
				
				stateBox.Text = "State: " + controller.State.ToString();
			}
		}
		
		void EndButtonClick(object sender, EventArgs e)
		{
			(controller as ControllableCodeController).End();
			RefreshState();
		}
		
		void StartButtonClick(object sender, EventArgs e)
		{
			(controller as ControllableCodeController).Start();
			RefreshState();
		}
		
		void PauseButtonClick(object sender, EventArgs e)
		{
			(controller as ControllableCodeController).Pause();
			RefreshState();
		}
		
		void RestoreButtonClick(object sender, EventArgs e)
		{
			(controller as ControllableCodeController).Restore();
			RefreshState();
		}
		
		public IConfiguration Configuration
		{
			get {return null;}
		}
	}
}
