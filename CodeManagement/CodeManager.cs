/*
 *
 * User: Expro
 * Date: 2010-06-30
 * Time: 18:21
 * 
 * 
 */
 
 //TODO: Runtime library modification checking
 //TODO: Patch system
 
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;
using System.Windows.Forms;
using CodeExecutionTools.Logging;
using CodeManagement.Definitions;
using Patterns;

namespace CodeManagement
{
	public class CodeManager: SharedCode, ISharedCodeProvider, IDisposablePattern
	{
		#region Fields
		private ISet<CodeDetails> codes; 
		private ISet<CodeDetails> modules;
		private ISet<CodeDetails> addins;
		private ISet<CodeDetails> shared;
		private ISet<string> conditions;
		private ISet<CodeDetails> controllable;
		private IDictionary<string, CodeDetails> resolver;
		private IDictionary<CodeDetails, CodeRepresentative> representatives;
		private IDictionary<CodeDetails, AppDomain> domains;
		
		private CodeDetails managerDetails;
		
		private AppDomain modulesDomain;
		
		private ISet<string> files;
		private string binaries;
		private MemoryMode memoryManagement;
		
		private bool areModulesLoaded;
		private SemaphoreSlim modulesLock;
		private bool separateModules;
		
		private PermissionSet modulePermissions;
		private PermissionSet addinPermissions;
		
		private bool disposed;
		#endregion
		
		#region Private Methods	
		private AppDomain CreateDomain(string name, PermissionSet permissions)
		{
			AppDomainSetup info = new AppDomainSetup();
			
			info.ApplicationBase = Environment.CurrentDirectory;
			info.PrivateBinPath = binaries;
			//FIXME: skonczyc
			permissions.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
			#if DEBUG
			permissions.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
			permissions.AddPermission(new SecurityPermission(PermissionState.Unrestricted));
			permissions.AddPermission(new TypeDescriptorPermission(PermissionState.Unrestricted));
			permissions.AddPermission(new EnvironmentPermission(PermissionState.Unrestricted));
			#else
			permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Environment.CurrentDirectory));
			permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread));
			#endif
			
			return AppDomain.CreateDomain(name, null, info, permissions, null);
		}
		
		private void UnloadDomain(CodeDetails details, int attempts = 3)
		{
			try
			{
				if (attempts != 0)
				{
					if (!details.Equals(managerDetails))
					{
						representatives[details].Destroy();
						AppDomain.Unload(domains[details]);
					}
				}
				else
					LoggingService.Trace.Warning("Unloading domain for " + details.ToString() + " failed during all attemps. Domain will be ignored.", new string[] {"CODE"}, this);
			}
			catch (CannotUnloadAppDomainException e)
			{
				--attempts;
				LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
				if (attempts != 0)
					LoggingService.Trace.Warning("Unloading domain for " + details.ToString() + " failed. Trying again (" + attempts.ToString() + " more attempts left).", new string[] {"CODE"}, this);
				
				UnloadDomain(details, attempts);
			}
			catch (AppDomainUnloadedException e)
			{
				LoggingService.Trace.Warning(e.ToString(), new string[] {"EXCEPTION"}, this);
				LoggingService.Trace.Warning("Detected attempt to unloaded domain. Removing data assosiated with referenced domain.", new string[] {"CODE"}, this);
				
				if (representatives.ContainsKey(details))
					representatives.Remove(details);
			}
			catch (Exception e)
			{
				LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
			}
			finally
			{
				if (domains.ContainsKey(details))
					domains.Remove(details);
			}
		}
		
		private AppDomain CreateModuleDomain(string name)
		{
			PermissionSet permissions = new PermissionSet(ModulePermissions);
			
			return CreateDomain(name, permissions);
		}
		
		private AppDomain CreateAddinDomain(string name)
		{
			PermissionSet permissions = new PermissionSet(AddinPermissions);
			
			return CreateDomain(name, permissions);
		}
		
		private void ScanFolder(string folder)
		{
			Contract.Requires(folder != null);
			
			if (Directory.Exists(folder))
			{
				string[] directoryFiles = Directory.GetFiles(folder, "*.dll");
				
				foreach (string directoryFile in directoryFiles)
					files.Add(directoryFile);
				
				foreach (string  subdirectory in Directory.GetDirectories(folder))
					ScanFolder(subdirectory);
			}
		}
		
		private void ScanForFiles()
		{
			files.Clear();
			
			foreach (string subdirectory in binaries.Split(new char[1] {';'}))
				ScanFolder(String.Format(@"{0}\{1}", Environment.CurrentDirectory, subdirectory));
		}
		
		
		private CodeDetails[] RetriveDetails(string file)
		{
			Contract.Requires(file != null);
			
			AppDomain analizeDomain;
			AppDomainSetup info = new AppDomainSetup();
			CodeAnalizer analizer;
			CodeDetails[] result;
			
			info.ApplicationBase = Environment.CurrentDirectory;
			info.PrivateBinPath = binaries;
			
			analizeDomain = AppDomain.CreateDomain("Analize Sandbox", null, info, ModulePermissions, null);
			
			analizer = (CodeAnalizer)analizeDomain.CreateInstanceAndUnwrap(typeof(CodeAnalizer).Assembly.FullName, typeof(CodeAnalizer).FullName);
			analizer.Path = file;
			analizer.Analize();
			
			result = new CodeDetails[analizer.Codes.Count];
			analizer.Codes.CopyTo(result, 0);
			
			AppDomain.Unload(analizeDomain);
			
			return result;
		}
		
		private void DistibuteToCategories(CodeDetails details)
		{
			Contract.Requires(details != null);
			
			ICollection<string> ifaces = details.Interfaces;
			
			if (details.ClassName.Equals(typeof(CodeManager).FullName))
				return;
			
			try
			{
				codes.Add(details);
				
				if (details.IsShared)
					shared.Add(details);
				
				if (details.IsModule)
					modules.Add(details);
				else
					addins.Add(details);
				
				if (details.IsControllable)
					controllable.Add(details);
				
				resolver.Add(details.ClassName, details);
				foreach (string iface in ifaces)
				{
					if (!resolver.ContainsKey(iface))
						resolver.Add(iface, details);
				}
			}
			catch (Exception e)
			{
				LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION", "RESOURCE"}, this);
				
				if (codes.Contains(details))
					codes.Remove(details);
				
				if (shared.Contains(details))
					shared.Remove(details);
				
				if (modules.Contains(details))
					modules.Remove(details);
						
				if (controllable.Contains(details))
					controllable.Remove(details);
			}
		}
		#endregion
		
		#region Protected Methods		
		protected internal CodeRepresentative ProvideRepresentative(CodeDetails details)
		{
			Contract.Requires(details != null);
			Contract.Ensures(Contract.Result<CodeRepresentative>() != null);
			
			AppDomain domain;
			CodeRepresentative result;
			CrossDomainLoggingAccessor loggingAccessor;
			
			if (!representatives.ContainsKey(details))
			{
				if (!details.IsModule)
					domain = CreateAddinDomain(String.Format("Domain for addin {0}.", details.Name));
				else
				{
					if (SeparateModules)
						domain = CreateModuleDomain(String.Format("Domain for module {0}.", details.Name));
					else
						domain = modulesDomain;
				}
				
				if (details.IsControllable)
					result = (CodeRepresentative)domain.CreateInstanceAndUnwrap(typeof(ControllableCodeRepresentative).Assembly.FullName, typeof(ControllableCodeRepresentative).FullName);
				else
					result = (CodeRepresentative)domain.CreateInstanceAndUnwrap(typeof(CodeRepresentative).Assembly.FullName, typeof(CodeRepresentative).FullName);
				 
				result.Details = details;
				result.Provider = this;
				loggingAccessor = (CrossDomainLoggingAccessor)domain.CreateInstanceAndUnwrap(typeof(CrossDomainLoggingAccessor).Assembly.FullName, typeof(CrossDomainLoggingAccessor).FullName);
				loggingAccessor.LocalDebugLoggingService.Parent = LoggingService.Debug;
				loggingAccessor.LocalTraceLoggingService.Parent = LoggingService.Trace;
				
				representatives.Add(details, result);
				domains.Add(details, domain);
			}
			else
				result = representatives[details];

			return result;
		}
		
		protected internal void Unload(CodeDetails details)
		{
			UnloadDomain(details);
			representatives.Remove(details);
		}
		
		protected internal CodeRepresentative GetRepresentative(CodeDetails details)
		{
			Contract.Requires(details != null);
			
			if (representatives.ContainsKey(details))
				return representatives[details];
			else
				return null;
		}
		
		protected virtual void OnProgress(string process, string processedItem, int step, int steps)
		{
			Contract.Requires(!String.IsNullOrEmpty(process));
			Contract.Requires(!String.IsNullOrEmpty(processedItem));
			Contract.Requires(step >= 0);
			Contract.Requires(steps >= 1);
			Contract.Requires(step < steps);
			
			if (Progress != null)
				Progress(this, new ProgressEventArgs(process, processedItem, step, steps));
		}
		#endregion
		
		#region Constuctors
		public CodeManager()
		{
			Author[] author = new Author[1];
			string[] description = new string[1];
			CodeRepresentative selfRepresentative = new CodeRepresentative();
			
			author[0] = new Author("Maciej 'Expro' Grabowski", "mds.expro@gmail.com");
			description[0] = "Dynamic code manager, extends application features with modules and plugins.";
			managerDetails = new CodeDetails(Application.ExecutablePath, typeof(CodeManager).FullName);
			managerDetails.Authors = author;
			managerDetails.Descriptions = description;
			managerDetails.IsModule = true;
			managerDetails.IsShared = true;
			managerDetails.Version = new CodeVersion(1, 0, 0);
			managerDetails.Name = "Code Manager";
			
			selfRepresentative.Details = managerDetails;
			selfRepresentative.Instance = this;
			selfRepresentative.Provider = this;
			selfRepresentative.State = CodeState.Created;

			modulePermissions = new PermissionSet(PermissionState.Unrestricted);
			
			addinPermissions = new PermissionSet(PermissionState.Unrestricted);
			addinPermissions.AddPermission(new UIPermission(PermissionState.Unrestricted));
			addinPermissions.AddPermission(new IsolatedStorageFilePermission(PermissionState.Unrestricted));
			
			SeparateModules = false;
			
			binaries = ";";
			memoryManagement = MemoryMode.FastestInitialization;

			codes = new SortedSet<CodeDetails>();
			shared = new SortedSet<CodeDetails>();
			modules = new SortedSet<CodeDetails>();
			addins = new SortedSet<CodeDetails>();
			controllable = new SortedSet<CodeDetails>();
			conditions = new SortedSet<string>();
			resolver = new SortedDictionary<string, CodeDetails>();
			representatives = new SortedDictionary<CodeDetails, CodeRepresentative>();
			domains = new SortedDictionary<CodeDetails, AppDomain>();
			files = new SortedSet<string>();
			modulesLock = new SemaphoreSlim(2);
			
			resolver.Add(typeof(CodeManager).FullName, managerDetails);
			domains.Add(managerDetails, AppDomain.CurrentDomain);
			representatives.Add(managerDetails, selfRepresentative);
			codes.Add(managerDetails);
			shared.Add(managerDetails);
			modules.Add(managerDetails);
		}
		
		~CodeManager()
		{
			Dispose(false);
		}
		#endregion
		
		#region Public Methods	
		public void AddCondition(string condition)
		{
			if (!String.IsNullOrWhiteSpace(condition))
				conditions.Add(condition.ToUpper());
		}
		
		public void Update()
		{
			CodeDetails[] details;
			int i = 0;
			
			ScanForFiles();
			
			foreach (string file in files)
			{
				OnProgress("Building code repository", file.Replace(Environment.CurrentDirectory, ""), i, files.Count);
				details = RetriveDetails(file);
				
				foreach (CodeDetails detail in details)
					DistibuteToCategories(detail);
				
				if (memoryManagement.Equals(MemoryMode.LowestMemoryUsage))
					GC.Collect();
				
				++i;
			}
			
			if (memoryManagement.Equals(MemoryMode.MostMemoryForApplication) || memoryManagement.Equals(MemoryMode.LowestMemoryUsage))
				GC.Collect();
		}
		
		public bool InitializeAndRunModules()
		{
			bool isSuccess = true;
			int i = 0;
			
			if (!SeparateModules)
				modulesDomain = CreateModuleDomain("Domain for all modules");
			else
				modulesDomain = null;
			
			foreach (CodeDetails module in modules)
			{
				OnProgress("Initialazing modules", module.ToString(), i, modules.Count);
				
				if (conditions.Contains(module.ModuleCondition))
				{
					if (!module.IsControllable)
						ProvideRepresentative(module).Initialize();
					else
						((ControllableCodeRepresentative)ProvideRepresentative(module)).Run();
					
					if (memoryManagement.Equals(MemoryMode.LowestMemoryUsage))
						GC.Collect();
				}
				
				++i;
			}

			foreach (CodeDetails module in modules)
			{
				if (!ProvideRepresentative(module).State.Equals(CodeState.Created) && conditions.Contains(module.ModuleCondition))
					isSuccess = false;
			}
			
			areModulesLoaded = isSuccess;
			
			if (areModulesLoaded)
				modulesLock.Wait();
			
			if (memoryManagement.Equals(MemoryMode.MostMemoryForApplication) || memoryManagement.Equals(MemoryMode.LowestMemoryUsage))
				GC.Collect();
			
			return areModulesLoaded;
		}
		
		public MarshalByRefObject ProvideSharedCode(string entityName)
		{
			Contract.Requires(entityName != null);
			
			CodeDetails details;
			CodeRepresentative representative;
			
			LoggingService.Trace.Information("Providing shared code: " + entityName, new string[] {"CODE"}, this);
			
			try
			{	
				details = resolver[entityName];
				if (!details.IsShared)
					throw new MissingSharedCodeException(entityName);
				
				representative = ProvideRepresentative(details);
				
				if (representative.Initialize())
					return (MarshalByRefObject)representative.Instance;
				else
					throw new MissingSharedCodeException(entityName);
			}
			catch (Exception e)
			{
				LoggingService.Trace.Error(e.ToString(), new string[] {"EXCEPTION"}, this);
				throw new MissingSharedCodeException(entityName);
			}
		}
		
		public void StopAndDestroyModules()
		{		
			modulesLock.Release();
		}
		
		public ICodeController GetController(CodeDetails details)
		{
			Contract.Requires(details != null);
			
			if (codes.Contains(details))
			{
				if (!details.IsControllable)
					return new CodeController(this, details);
				else
					return new ControllableCodeController(this, details);
			}
			else
				return null;
		}
		
		public void WaitForModulesDestruction()
		{
			modulesLock.Wait();
			modulesLock.Wait();
		}
		
		public override sealed void Dispose()
		{
			if (!Disposed)
				Dispose(true);
		}
		
		public bool Disposed
		{
			get {return disposed;}
		}
		
		public void CheckDisposed()
		{
			if (Disposed)
				throw new ObjectDisposedException(ToString());
		}
		
		public void Dispose(bool disposeManagedResources)
		{
			int i = 0;
			
			if (!Disposed)
			{
				try
				{
					foreach (CodeDetails details in representatives.Keys)
					{
						try
						{
							if (disposeManagedResources)
								OnProgress("Disposing resources", details.ToString(), i++, representatives.Count);
							UnloadDomain(details);
						}
						catch (InvalidOperationException)
						{
							Dispose(disposeManagedResources);
						}
					}
					
					modulesLock.Dispose();
			
					GC.SuppressFinalize(this);
					disposed = true;
				}
				catch (Exception)
				{
					Environment.Exit(1);
				}
			}
		}
		#endregion
		
		#region Properties
		public string Binaries
		{
			get {return binaries;}
			set {binaries = value;}
		}
		
		public MemoryMode MemoryManagement
		{
			get {return memoryManagement;}
			set {memoryManagement = value;}
		}
		
		public ICollection<CodeDetails> Codes
		{
			get {return codes;}
		}
		
		public ICollection<CodeDetails> Modules
		{
			get {return modules;}
		}
		
		public ICollection<CodeDetails> Shared
		{
			get {return shared;}
		}
		
		public ICollection<CodeDetails> Controllable
		{
			get {return controllable;}
		}
		
		public ICollection<CodeDetails> Addins
		{
			get {return addins;}
		}
		
		public bool SeparateModules
		{
			get {return separateModules;}
			
			set {separateModules = value;}
		}

		public PermissionSet AddinPermissions
		{
			get {return addinPermissions;}
			set {addinPermissions = value;}
		}
		
		public PermissionSet ModulePermissions
		{
			get {return modulePermissions;}
			set {modulePermissions = value;}
		}
		#endregion
		
		#region Events
		public event EventHandler<ProgressEventArgs> Progress;
		#endregion
	}
}
