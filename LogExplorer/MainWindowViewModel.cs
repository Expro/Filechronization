/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-02
 * Time: 19:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using CodeExecutionTools.Logging;
using Patterns;

namespace LogExplorer
{
	public class MainWindowViewModel
	{
		private ICollection<LogEntry> filteredEntries;
		private ICollection<string> filteredTags;
		private ICollection<string> filteredSenders;
		private bool includeInformations;
		private bool includeWarnings;
		private bool includeErrors;
		private ICollection<string> selectedTags;
		private ICollection<string> selectedSenders;
		private string file;
		private bool isLoading;
		
		public MainWindowViewModel()
		{
			this.filteredEntries = new HashSet<LogEntry>();
			this.filteredTags = new HashSet<string>();
			this.filteredSenders = new HashSet<string>();
			this.includeErrors = true;
			this.includeInformations = true;
			this.includeWarnings = true;
			this.selectedSenders = new HashSet<string>();
			this.selectedTags = new HashSet<string>();
			this.isLoading = false;
		}
		
		public bool IsLoading
		{
			get {return isLoading;}
		}
		
		public void IndicateLoadProgress(object sender, ProgressEventArgs e)
		{
			if (LoadProgress != null)
				LoadProgress(sender, e);
		}
		
		public string File
		{
			get {return file;}
			set
			{
				file = value;
				
				if (FileChanged != null)
				{
					isLoading = true;
					FileChanged(this, EventArgs.Empty);
					isLoading = false;
				}
			}
		}
		
		public ICollection<string> SelectedSenders
		{
			get {return selectedSenders;}
			set
			{
				selectedSenders = value;
				
				if (SelectedSendersChanged != null)
					SelectedSendersChanged(this, EventArgs.Empty);
			}
		}	
		
		public ICollection<string> SelectedTags
		{
			get {return selectedTags;}
			set
			{
				selectedTags = value;
				
				if (SelectedTagsChanged != null)
					SelectedTagsChanged(this, EventArgs.Empty);
			}
		}
		
		public bool IncludeErrors
		{
			get {return includeErrors;}
			set
			{
				includeErrors = value;
				
				if (IncludeErrorsChanged != null)
					IncludeErrorsChanged(this, EventArgs.Empty);
			}
		}
		
		public bool IncludeWarnings
		{
			get {return includeWarnings;}
			set
			{
				includeWarnings = value;
				
				if (IncludeWarningsChanged != null)
					IncludeWarningsChanged(this, EventArgs.Empty);
			}
		}
		
		public bool IncludeInformations
		{
			get {return includeInformations;}
			set
			{
				includeInformations = value;
				
				if (IncludeInformationsChanged != null)
					IncludeInformationsChanged(this, EventArgs.Empty);
			}
		}
		
		public ICollection<string> FilteredSenders
		{
			get {return filteredSenders;}
			set
			{
				filteredSenders = value;
				selectedSenders = value;
				
				if (FilteredSendersChanged != null)
					FilteredSendersChanged(this, EventArgs.Empty);
			}
		}
		
		public ICollection<string> FilteredTags
		{
			get {return filteredTags;}
			set
			{
				filteredTags = value;
				selectedTags = value;
				
				if (FilteredTagsChanged != null)
					FilteredTagsChanged(this, EventArgs.Empty);
			}
		}
		
		public ICollection<LogEntry> FilteredEntries
		{
			get {return filteredEntries;}
			set
			{
				filteredEntries = value;
				
				if (FilteredEntriesChanged != null)
					FilteredEntriesChanged(this, EventArgs.Empty);
			}
		}
		
		public event EventHandler FilteredEntriesChanged;
		public event EventHandler SelectedSendersChanged;
		public event EventHandler FilteredTagsChanged;
		public event EventHandler FilteredSendersChanged;
		public event EventHandler IncludeInformationsChanged;
		public event EventHandler IncludeWarningsChanged;
		public event EventHandler IncludeErrorsChanged;
		public event EventHandler SelectedTagsChanged;
		public event EventHandler FileChanged;
		public event EventHandler<ProgressEventArgs> LoadProgress;
	}
}
