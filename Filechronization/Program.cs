/*
 *
 * User: Expro
 * Date: 2010-07-17
 * Time: 22:29
 * 
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

using CodeExecutionTools.Logging;
using CodeManagement;
using Patterns;

namespace Filechronization
{
	#region Comment
	/// <summary>
	/// Description of Program.
	/// </summary>
	#endregion
	internal sealed class Program
	{	
		#region Comment
		/// <summary>
		/// Program entry point.
		/// </summary>
		#endregion
		[MTAThread]
		private static void Main(string[] args)
		{
			string workString;
			SplashScreen splash;
			Semaphore splashLock;
			CodeManager manager;
			XMLZipLogFileHandler logFile;
			
			manager = new CodeManager();
			if (!Directory.Exists(Environment.CurrentDirectory +"\\Logs\\"))
				Directory.CreateDirectory(Environment.CurrentDirectory +"\\Logs\\");
			logFile = new XMLZipLogFileHandler(Environment.CurrentDirectory +"\\Logs\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fff") + ".log", false);
			
			try
			{
				splashLock = new Semaphore(0, 1);
				
				#if DEBUG
				manager.AddCondition("DEBUG");
				#endif
				//manager.AddCondition("GUI");
				
				LoggingService.Debug.LogHandlers.Add(logFile);
				LoggingService.Trace.LogHandlers.Add(logFile);
				
				manager.Binaries += "Modules;";
				manager.Binaries += "Addins;";			
				manager.Binaries += "Libraries;";
				
				manager.Progress += delegate(object sender, ProgressEventArgs e) {LoggingService.Trace.Log(e.Process + ": " + e.ProcessedItem, new string[] {"APPLICATION", "CODE"});};
					
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				
				LoggingService.Trace.Log("Current directory: " + Environment.CurrentDirectory, new string[] {"ENVIRONMENT"});
				LoggingService.Trace.Log("Machine name: " + Environment.MachineName, new string[] {"ENVIRONMENT"});
				LoggingService.Trace.Log("Operating system: " + Environment.OSVersion, new string[] {"ENVIRONMENT"});
				LoggingService.Trace.Log("Is operating system 64bit: " + Environment.Is64BitOperatingSystem.ToString(), new string[] {"ENVIRONMENT"});
				LoggingService.Trace.Log("Processors: " + Environment.ProcessorCount.ToString(), new string[] {"ENVIRONMENT"});
				LoggingService.Trace.Log("Is processor 64bit: " + Environment.Is64BitProcess.ToString(), new string[] {"ENVIRONMENT"});
	
				workString = "Environment variables:";
				
				foreach (DictionaryEntry element in Environment.GetEnvironmentVariables())
					workString += Environment.NewLine + element.Key.ToString() + ": " + element.Value.ToString();
				LoggingService.Trace.Log(workString, new string[] {"ENVIRONMENT"});
				
				workString = "Command line arguments:";
				
				foreach (string argument in Environment.GetCommandLineArgs())
					workString += Environment.NewLine + argument;
				LoggingService.Trace.Log(workString, new string[] {"ENVIRONMENT"});
				
				LoggingService.Trace.Log("Core initialized.", new string[] {"APPLICATION", "CODE"});
				
				splash = new SplashScreen(splashLock);
				ThreadPool.QueueUserWorkItem(delegate(object o) {Application.Run(splash);});
				splashLock.WaitOne();
				manager.Progress += splash.ProgressHandler;
				
				LoggingService.Trace.Log("Searching for available assemblies...", new string[] {"APPLICATION", "CODE"});
				manager.Update();
				LoggingService.Trace.Log("Search completed.", new string[] {"APPLICATION", "CODE"});
				
				LoggingService.Trace.Log("Initializing modules...", new string[] {"APPLICATION", "CODE"});
				if (manager.InitializeAndRunModules())
					LoggingService.Trace.Log("Initialization completed.", new string[] {"APPLICATION", "CODE"});
				else
					LoggingService.Trace.Log("Initialization failed.", new string[] {"APPLICATION", "CODE"});

				manager.Progress -= splash.ProgressHandler;
				splash.Invoke(new MethodInvoker(delegate() {splash.Close();}));
				manager.WaitForModulesDestruction();
				LoggingService.Trace.Log("Shuting down...", new string[] {"APPLICATION", "CODE"});
				LoggingService.Trace.Log("Disposing Code Manager resources...", new string[] {"APPLICATION", "CODE"});
				manager.Dispose();
				LoggingService.Trace.Log("Shut down completed.", new string[] {"APPLICATION", "CODE"});
			}
			catch (Exception e)
			{
				LoggingService.Trace.Error("Emergency shut down due to error: " + e.ToString(), new string[] {"APPLICATION", "CODE"});
			}
			finally
			{
				if (manager != null)
				{
					if (!manager.Disposed)
						manager.Dispose();
				}
				
				if (logFile != null)
				{
					if (!logFile.Disposed)
						logFile.Dispose();
				}
			}
		}
	}
}
