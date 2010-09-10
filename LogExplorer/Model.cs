/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-31
 * Time: 22:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeExecutionTools.Logging;
using Patterns;

namespace LogExplorer
{
	public class Model
	{	
		private ICollection<LogEntry> entries;
		private ICollection<string> tags;
		private ICollection<string> senders;
		private MainWindowViewModel viewModel;
		
		public Model(MainWindowViewModel viewModel)
		{
			if (viewModel == null)
				throw new ArgumentNullException("viewModel");
			
			this.entries = new HashSet<LogEntry>();
			this.senders = new HashSet<string>();
			this.tags = new HashSet<string>();
			
			viewModel.FileChanged += new EventHandler(FileChangedHandler);
			
			this.viewModel = viewModel;
			this.viewModel.SelectedSendersChanged += DoRefilter;
			this.viewModel.SelectedTagsChanged += DoRefilter;
			this.viewModel.IncludeInformationsChanged += DoRefilter;
			this.viewModel.IncludeWarningsChanged += DoRefilter;
			this.viewModel.IncludeErrorsChanged += DoRefilter;
		}
		
		private void DoRefilter(object sender, EventArgs e)
		{
			Filter();
		}

		private void Filter()
		{
			ICollection<LogEntry> result = new HashSet<LogEntry>();
			bool categoryFits = false;
			int i = 0;
			
			foreach (LogEntry entry in entries)
			{
				viewModel.IndicateLoadProgress(this, new ProgressEventArgs("Filtering", i.ToString() + " of " + entries.Count.ToString(), i++, entries.Count));
				
				switch (entry.Category)
				{
					case EntryCategory.Information:
						categoryFits = viewModel.IncludeInformations;
						break;
					case EntryCategory.Warning:
						categoryFits = viewModel.IncludeWarnings;
						break;
					case EntryCategory.Error:
						categoryFits = viewModel.IncludeErrors;
						break;
					default:
						throw new Exception("Invalid value for EntryCategory");
				}
				
				if (categoryFits)
				{
					if (viewModel.SelectedSenders.Contains(entry.Sender))
					{
						foreach (string tag in entry.Tags)
						{
							if (viewModel.SelectedTags.Contains(tag))
							{
								result.Add(entry);
								break;
							}
						}
					}
				}
			}
			
			viewModel.FilteredEntries = result;
		}

		private void FileChangedHandler(object sender, EventArgs e)
		{
			XMLZipLogFileHandler handler;
			
			if (viewModel.File != null)
			{
				try
				{
					handler = new XMLZipLogFileHandler(viewModel.File, true);
					handler.Progress += viewModel.IndicateLoadProgress;
					entries = handler.Read();
					
					handler.Dispose();
					
					foreach (LogEntry entry in entries)
					{
						if (!senders.Contains(entry.Sender))
							senders.Add(entry.Sender);
						
						foreach (string tag in entry.Tags)
						{
							if (!tags.Contains(tag))
								tags.Add(tag);
						}
					}
					
					viewModel.FilteredEntries = entries;
					viewModel.FilteredSenders = senders;
					viewModel.FilteredTags = tags;
					
					if (handler != null)
						handler.Progress -= viewModel.IndicateLoadProgress;
				}
				catch (Exception exception)
				{
					LoggingService.Debug.Log(exception.ToString(), null, this, EntryCategory.Error);
				}
			}
		}
	}
}
