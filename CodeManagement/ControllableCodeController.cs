/*
 *
 * User: Expro
 * Date: 2010-07-31
 * Time: 18:11
 * 
 * 
 */
using System;
using System.Diagnostics.Contracts;
using System.Security;
using System.Security.Permissions;

namespace CodeManagement
{
	
	public class ControllableCodeController: CodeController
	{
		#region Protected Properies
		protected ControllableCodeRepresentative ControllableRepresentative
		{
			get {return Representative as ControllableCodeRepresentative;}
			set {Representative = value;}
		}
		#endregion
		
		#region Constuctor
		public ControllableCodeController(CodeManager manager, CodeDetails details): base(manager, details)
		{
		}
		#endregion
		
		#region Public Methods
		
		[ReflectionPermission(SecurityAction.Demand)]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		public override bool Create()
		{
			ControllableRepresentative.Create();
			
			return State.Equals(CodeState.Created);
		}
		
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		public override bool Destroy()
		{
			ControllableRepresentative.Destroy();
			
			return State.Equals(CodeState.Loaded);
		}
		
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		public bool Start()
		{
			if (State.Equals(CodeState.Created))
			{
				if (ControllableRepresentative.StateOfControllable.Equals(ControllableState.Stopped))
					ControllableRepresentative.Start();
				
				return ControllableRepresentative.StateOfControllable.Equals(ControllableState.Started);
			}
			else
				return false;
		}
		
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		public bool Pause()
		{
			if (State.Equals(CodeState.Created))
			{
				if (ControllableRepresentative.StateOfControllable.Equals(ControllableState.Started))
					ControllableRepresentative.Pause();
				
				return ControllableRepresentative.StateOfControllable.Equals(ControllableState.Paused);
			}
			else
				return false;
		}
		
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		public bool Restore()
		{
			if (State.Equals(CodeState.Created))
			{
				if (ControllableRepresentative.StateOfControllable.Equals(ControllableState.Paused))
					ControllableRepresentative.Restore();
				
				return ControllableRepresentative.StateOfControllable.Equals(ControllableState.Started);
			}
			else
				return false;
		}
		
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		public bool End()
		{
			if (State.Equals(CodeState.Created))
			{
				if (ControllableRepresentative.StateOfControllable.Equals(ControllableState.Started))
					ControllableRepresentative.End();
				
				return ControllableRepresentative.StateOfControllable.Equals(ControllableState.Stopped);
			}
			else
				return false;
		}
		
		
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Execution)]
		public bool Run()
		{
			if (State.Equals(CodeState.Unloaded))
				Load();
			
			ControllableRepresentative.Run();
			
			if (ControllableRepresentative != null)
				return ControllableRepresentative.StateOfControllable.Equals(ControllableState.Started);
			else
				return false;
		}
		#endregion
		
		#region Properties
		public ControllableState StateOfControllable
		{
			get
			{
				if (ControllableRepresentative != null)
					return ControllableRepresentative.StateOfControllable;
				else
					return ControllableState.Stopped;
			}
		}
		#endregion
	}
}
