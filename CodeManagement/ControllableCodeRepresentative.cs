/*
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
		#region Fields
		private Thread thread;
		private ControllableState stateOfControllable;
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
		}
		#endregion
		
		#region Public Methods
		public new bool Create()
		{	
			bool result = false;
			
			try
			{
				result = base.Create();
				if (result)
					thread = new Thread(delegate()
					                    {
					                    	try
					                    	{
					                    		Instance.Start();
					                    	}
					                    	catch (Exception e)
					                    	{
					                    		LoggingService.Trace.Log(e.ToString(), new string[] {"EXCEPTION"}, this, EntryCategory.Error);
					                    	}
					                    });
			}
			catch (Exception e)
			{
				LoggingService.Trace.Log(e.ToString(), new string[] {"EXCEPTION"}, this, EntryCategory.Error);
				State = CodeState.Corrupted;
			}
				
			return result;
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
					LoggingService.Trace.Log(e.ToString(), new string[] {"EXCEPTION"}, this, EntryCategory.Error);
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
					thread.Start();	
					stateOfControllable = ControllableState.Started;
				}
				catch (Exception e)
				{
					LoggingService.Trace.Log(e.ToString(), new string[] {"EXCEPTION"}, this, EntryCategory.Error);
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
					Instance.Pause();
					stateOfControllable = ControllableState.Paused;
				}
				catch (Exception e)
				{
					LoggingService.Trace.Log(e.ToString(), new string[] {"EXCEPTION"}, this, EntryCategory.Error);
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
					Instance.Restore();
					stateOfControllable = ControllableState.Started;
				}
				catch (Exception e)
				{
					LoggingService.Trace.Log(e.ToString(), new string[] {"EXCEPTION"}, this, EntryCategory.Error);
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
					Instance.End();
					thread.Abort();
					stateOfControllable = ControllableState.Stopped;
				}
				catch (Exception e)
				{
					LoggingService.Trace.Log(e.ToString(), new string[] {"EXCEPTION"}, this, EntryCategory.Error);
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
