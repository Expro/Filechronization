/*
 *
 * User: Expro
 * Date: 2010-07-29
 * Time: 12:56
 * 
 * 
 */
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security;
using System.Security.Permissions;
using CodeExecutionTools.Logging;

namespace CodeManagement
{
	
	public class CodeController: MarshalByRefObject, ICodeController
	{
		#region Fields
		private CodeManager manager;
		private CodeRepresentative representative;
		private CodeDetails details;
		#endregion
		
		#region Protected Properties
		protected virtual CodeRepresentative Representative
		{
			
			get {return representative;}
			
			set {representative = value;}
		}
		#endregion
		
		#region Constuctors
		
		public CodeController(CodeManager manager, CodeDetails details)
		{
			Contract.Requires(manager != null);
			Contract.Requires(details != null);
			
			this.manager = manager;
			this.details = details;
			this.representative = manager.GetRepresentative(details);
		}
		#endregion
		
		#region Public Methods
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlAppDomain)]
		[FileIOPermission(SecurityAction.Demand)]
		[ReflectionPermission(SecurityAction.Demand)]
		public bool Load()
		{
			Contract.Ensures(representative != null);
			
			try
			{
				if (State.Equals(CodeState.Unloaded))
					Representative = manager.ProvideRepresentative(details);
			
				if (Representative != null)
					Representative.Load();
			}
			catch (Exception e)
			{
				LoggingService.Trace.Log(e.ToString(), new string[] {"EXCEPTION"}, this, EntryCategory.Error);
			}
				
			return State.Equals(CodeState.Loaded);
		}
		
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		[ReflectionPermission(SecurityAction.Demand)]
		public virtual bool Create()
		{
			try
			{
				if (State.Equals(CodeState.Loaded))
					Representative.Create();
			}
			catch (Exception e)
			{
				LoggingService.Trace.Log(e.ToString(), new string[] {"EXCEPTION"}, this, EntryCategory.Error);
			}
			
			return State.Equals(CodeState.Created);
		}
		
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		public virtual bool Destroy()
		{
			try
			{
				if (State.Equals(CodeState.Created))
					Representative.Destroy();
			}
			catch (Exception e)
			{
				LoggingService.Trace.Log(e.ToString(), new string[] {"EXCEPTION"}, this, EntryCategory.Error);
			}
			
			return State.Equals(CodeState.Loaded);
		}
		
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		public virtual bool Unload()
		{
			if (!State.Equals(CodeState.Corrupted) && !State.Equals(CodeState.Unloaded))
			{
				if (State.Equals(CodeState.Created))
					Representative.Destroy();
				
				Representative = null;
				
				if (State.Equals(CodeState.Loaded))
				    manager.Unload(details);
			}
			
			return State.Equals(CodeState.Unloaded);
		}
		
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		public bool Initialize()
		{
			if (State.Equals(CodeState.Unloaded))
				Load();
			
			representative.Initialize();
			
			return State.Equals(CodeState.Created);
		}
		#endregion
		
		#region Properties
		public CodeState State
		{
			
			get
			{
				if (representative == null)
					return CodeState.Unloaded;
				else
					return Representative.State;
			}
		}
		#endregion
	}	
}
