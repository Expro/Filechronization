﻿/*
 *
 * User: Expro
 * Date: 2010-07-11s
 * Time: 20:02
 * 
 * 
 */
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

using CodeExecutionTools.Logging;
using CodeManagement.Definitions;

namespace CodeManagement
{
	using Definitions;

	#region Comment
	/// <summary>
	/// 	Provides cross-domain control over code.
	/// </summary>
	#endregion
	public class CodeRepresentative: MarshalByRefObject
	{	
		public const int Timeout = 5000;
		
		#region Fields
		private ICode instance;
		private CodeDetails details;
		private ISharedCodeProvider provider;
		private CodeState state;
		private Type type;
		
		private SemaphoreSlim instanceLock;
		#endregion

		#region Private Methods	
		private ICode[] ProvideParameters(ConstructorInfo constructor)
		{
			Contract.Requires(constructor != null);
			
			ParameterInfo[] parameters = constructor.GetParameters();
			ICode[] result = new ICode[parameters.Length];
			int i = 0;
			
			foreach (ParameterInfo parameter in parameters)
			{
				try
				{
					result[i] = provider.ProvideSharedCode(parameter.ParameterType.FullName);
				}
				catch (MissingSharedCodeException e)
				{
					LoggingService.Trace.Warning(e.ToString(), new string[] {"EXCEPTION", "CODE"}, this);
					result[i] = null;
				}
					
				++i;
			}
			
			return result;
		}
		#endregion	
		
		#region Protected Properties
		protected internal ICode Instance
		{		
			get
			{
				ICode result;
				
				instanceLock.Wait();
				result = instance;
				instanceLock.Release();
				
				return result;
			}
				
			set
			{
				instanceLock.Wait();
				if (!state.Equals(CodeState.Corrupted))
					instance = value;
				instanceLock.Release();
			}
		}
		#endregion
		
		#region Constructors
		public CodeRepresentative()
		{
			this.state = CodeState.Unloaded;
			this.instanceLock = new SemaphoreSlim(1);
		}
		#endregion
		
		#region Public Methods	
		public bool Load()
		{
			Type[] types;
			
			if (state.Equals(CodeState.Unloaded))
			{
				state = CodeState.Loading;
				
				LoggingService.Trace.Information("Loading code: " + details.ToString(), new string[] {"CODE"}, this);
				
				try
				{
					types = Assembly.LoadFrom(details.File).GetExportedTypes();
					
					foreach (Type assemblyType in types)
					{
						if (assemblyType.FullName.Equals(details.ClassName))
						{
							type = assemblyType;
							break;
						}
					}
					
					if (type == null)
						state = CodeState.Corrupted;
					else
						state = CodeState.Loaded;
				}
				catch (Exception e)
				{
					LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
					state = CodeState.Corrupted;
				}
			}
			
			return state.Equals(CodeState.Loaded);
		}
		
		
		public bool Create()
		{
			ConstructorInfo[] constructors;
			object[] parameters;
			Thread creationThread;
			
			if (state.Equals(CodeState.Loaded))
			{
				state = CodeState.Creating;
				
				LoggingService.Trace.Information("Creating code: " + details.ToString(), new string[] {"CODE"}, this);
				
				try
				{
					constructors = type.GetConstructors();
					
					foreach (ConstructorInfo constructor in constructors)
					{
						parameters = ProvideParameters(constructor);
						
						creationThread = new Thread(() =>
						                            {
						                            	try
						                            	{
						                            		Instance = (ICode)constructor.Invoke(parameters);
						                            	}
						                            	catch (ThreadAbortException)
						                            	{
						                            		LoggingService.Trace.Information("Creation of code: " + details.ToString() + " was aborted due to timeout", new string[] {"CODE"}, this);
						                            	}
						                            });
						
						creationThread.Start();
						creationThread.Join(Timeout);
						
						if (creationThread.ThreadState == System.Threading.ThreadState.Running)
							creationThread.Abort();
						
						instanceLock.Wait();
						if (instance != null)
						{
							state = CodeState.Created;
							instanceLock.Release();

							break;
						}
						else
							state = CodeState.Corrupted;
						instanceLock.Release();
					}
				}
				catch (Exception e)
				{
					LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
					
					state = CodeState.Corrupted;
				}
				finally
				{
					instanceLock.Release();
				}
			}
			
			return state.Equals(CodeState.Created) && (instance != null);
		}
		
		
		public bool Destroy()
		{
			if (state.Equals(CodeState.Created))
			{
				try
				{
					state = CodeState.Destroying;
					
					LoggingService.Trace.Information("Destroying code: " + details.ToString(), new string[] {"CODE"}, this);
					
					instance.Dispose();
					instance = null;
				
					GC.Collect();
					state = CodeState.Loaded;
				}
				catch (ObjectDisposedException)
				{
					instance = null;
				
					GC.Collect();
					state = CodeState.Loaded;
				}
				catch (Exception e)
				{
					LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
				}
			}
			
			return state.Equals(CodeState.Loaded);
		}
		
		#region Comment
		/// <summary>
		/// 	Shortcut for object creation from any state.
		/// </summary>
		#endregion
		public virtual bool Initialize()
		{
			if (state.Equals(CodeState.Unloaded))
				Load();
			
			if (state.Equals(CodeState.Loaded))
				Create();
			
			return state.Equals(CodeState.Created);
		}
		#endregion

		#region Properties
		public CodeDetails Details
		{
			get {return details;}
			protected internal set {details = value;}
		}
		
		public ISharedCodeProvider Provider
		{
			get {return provider;}
			protected internal set {provider = value;}
		}
		
		public CodeState State
		{
			get {return state;}
			internal set {state = value;}
		}
		#endregion		
	}
}
 