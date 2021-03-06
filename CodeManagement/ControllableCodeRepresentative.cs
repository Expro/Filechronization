﻿/*
 *
 * User: Expro
 * Date: 2010-07-22
 * Time: 15:05
 * 
 * 
 */
using System;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Threading;

using CodeExecutionTools.Logging;

namespace CodeManagement
{
	using Definitions;

	#region Comment
	/// <summary>
	/// 	Extended code representative, provides access to control methods.
	/// </summary>
	#endregion
	
	public class ControllableCodeRepresentative: CodeRepresentative
	{
		const int END_TIMEOUT = 5000;
		
		#region Fields
		private Thread thread;
		private ControllableState stateOfControllable;
		private SemaphoreSlim startBlocker;
		private SemaphoreSlim createIndicator;
		#endregion
		
		#region Protected Properties
		protected new IControllableCode Instance
		{
			
			get {return (IControllableCode)base.Instance;}
			
			set {base.Instance = value;}
		}
		#endregion
		
		#region Constructors	
		public ControllableCodeRepresentative(): base()
		{
			this.stateOfControllable = ControllableState.Stopped;
			this.startBlocker = new SemaphoreSlim(0);
			this.createIndicator = new SemaphoreSlim(0);
		}
		#endregion
		
		#region Public Methods
		public new bool Create()
		{	
			try
			{
				thread = new Thread(() =>
				                    {
				                    	try
				                    	{
				                    		base.Create();
				                    		createIndicator.Release();
				                    		startBlocker.Wait();
				                    		Instance.Start();
				                    		
				                    		stateOfControllable = ControllableState.Stopped;
				                    		LoggingService.Trace.Information("Finished execution of controllable code: " + Details.ToString(), new string[] {"CODE"}, this);
				                    	}
				                    	catch (ThreadAbortException)
				                    	{
				                    		LoggingService.Trace.Information("Execution of " + Details.ToString() + " was aborted", new string[] {"CODE"}, this);
				                    	}
				                    	catch (Exception e)
				                    	{
				                    		LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
				                    	}
				                    });
				thread.Start();
				createIndicator.Wait();
				return true;
			}
			catch (Exception e)
			{
				LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
				State = CodeState.Corrupted;
			}
				
			return false;
		}
		
		
		public new bool Destroy()
		{
			bool result = false;
			
			if (!stateOfControllable.Equals(ControllableState.Stopped))
			{
				try
				{
					End();
				
					result = base.Destroy();
				}
				catch (Exception e)
				{
					LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
					State = CodeState.Corrupted;
				}
			}
				
			return result;
		}
		
		
		public void Start()
		{
			if (State.Equals(CodeState.Created))
			{
				try
				{
					LoggingService.Trace.Information("Starting controllable code: " + Details.ToString(), new string[] {"CODE"}, this);

					startBlocker.Release();
					stateOfControllable = ControllableState.Started;
				}
				catch (Exception e)
				{
					LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
					State = CodeState.Corrupted;
				}
			}
		}
		
		
		public void Pause()
		{
			if (State.Equals(CodeState.Created))
			{
				try
				{
					LoggingService.Trace.Information("Pausing controllable code: " + Details.ToString(), new string[] {"CODE"}, this);                 		

					Instance.Pause();
					stateOfControllable = ControllableState.Paused;
				}
				catch (Exception e)
				{
					LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
					State = CodeState.Corrupted;
				}
			}
		}

		
		public void Restore()
		{
			if (State.Equals(CodeState.Created))
			{
				try
				{
					LoggingService.Trace.Information("Restoring controllable code: " + Details.ToString(), new string[] {"CODE"}, this);
					
					Instance.Restore();
					stateOfControllable = ControllableState.Started;
				}
				catch (Exception e)
				{
					LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
					State = CodeState.Corrupted;
				}
			}
		}
		
		
		public void End()
		{
			if (State.Equals(CodeState.Created))
			{
				try
				{
					LoggingService.Trace.Information("Ending controllable code: " + Details.ToString(), new string[] {"CODE"}, this);

					Instance.End();
					thread.Abort();
					thread.Join(END_TIMEOUT);
					stateOfControllable = ControllableState.Stopped;
				}
				catch (Exception e)
				{
					LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
					State = CodeState.Corrupted;
				}
			}
		}
		
		#region Comment
		/// <summary>
		/// 	Shortcut for creation and start of controllable code.
		/// </summary>
		/// <returns>
		/// 	True if code successfully started.
		/// </returns>
		#endregion	
		public bool Run()
		{
			if (State.Equals(CodeState.Unloaded))
				Load();
			
			if (State.Equals(CodeState.Loaded))
				Create();
			
			switch (stateOfControllable)
			{
				case ControllableState.Stopped:
					Start();
				break;
				case ControllableState.Paused:
					Restore();
				break;
			}
			
			return stateOfControllable.Equals(ControllableState.Started);
		}
		#endregion
		
		#region Properties
		public ControllableState StateOfControllable
		{
			get {return stateOfControllable;}
		}
		#endregion
	}
}
