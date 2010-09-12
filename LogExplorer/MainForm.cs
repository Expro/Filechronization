/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-08-31
 * Time: 15:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeExecutionTools.Logging;
using Patterns;

namespace LogExplorer
{
	public partial class MainForm: Form
	{	
		private MainWindowViewModel viewModel;
		private ProgressForm progressForm;

		private void FilteredTagsChangedHandler(object sender, EventArgs e)
		{
			int i = 0;
			tags.BeginUpdate();
			
			tags.Items.Clear();
			foreach (string tag in viewModel.FilteredTags)
			{
				AddTag(tag);
				viewModel.IndicateLoadProgress(this, new ProgressEventArgs("Creating display", "Tags", i++, viewModel.FilteredTags.Count));
			}
				
			tags.EndUpdate();
		}

		private void FilteredSendersChangedHandler(object sender, EventArgs e)
		{
			int i = 0;
			senders.BeginUpdate();
			
			senders.Items.Clear();
			foreach (string logSender in viewModel.FilteredSenders)
			{
				AddSender(logSender);
				viewModel.IndicateLoadProgress(this, new ProgressEventArgs("Creating display", "Senders", i++, viewModel.FilteredSenders.Count));
			}
			
			senders.EndUpdate();
		}

		private void FilteredEntriesChangedHandler(object sender, EventArgs e)
		{
			int i = 0;
			logTree.BeginUpdate();
			
			logTree.Nodes.Clear();
			foreach (LogEntry entry in viewModel.FilteredEntries)
			{
				AddEntry(entry);
				viewModel.IndicateLoadProgress(this, new ProgressEventArgs("Creating display", "Entries", i++, viewModel.FilteredEntries.Count));
			}
			
			logTree.EndUpdate();
		}

		private void FileChangedHandler(object sender, EventArgs e)
		{
			Text = "Log Explorer - " + viewModel.File;
			typeInformations.Checked = true;
			typeWarnings.Checked = true;
			typeErrors.Checked = true;
			copyAllMenu.Enabled = false;
			copyMessageMenu.Enabled = false;
		}

		private void AddEntry(LogEntry entry)
		{
			string entryString = entry.ToString();
			int breakPoint = entryString.IndexOf(Environment.NewLine);
			string[] lines;
			TreeNode subnode;
			TreeNode node = new TreeNode(entryString.Substring(0, (breakPoint >= 0)?breakPoint:entryString.Length));
			node.Tag = entry;

			switch (entry.Category)
			{
				case EntryCategory.Information:
					node.ImageKey = "information.png";
					node.ForeColor = Color.Green;
					break;
				case EntryCategory.Warning:
					node.ImageKey = "warnings.png";
					node.ForeColor = Color.DarkGoldenrod;
					break;
				case EntryCategory.Error:
					node.ImageKey = "errors.png";
					node.ForeColor = Color.Red;
					break;
				default:
					throw new Exception("Invalid value for EntryCategory");
			}

			if (breakPoint >= 0)
			{
				lines = entryString.Remove(0, breakPoint).Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
				
				foreach (string line in lines)
				{
					subnode = new TreeNode(line);
					subnode.ImageKey = node.ImageKey;
					subnode.ForeColor = node.ForeColor;
					node.Nodes.Add(subnode);
				}
			}
			
			logTree.Nodes.Add(node);
		}

		private void AddTag(string tag)
		{
			if (!String.IsNullOrEmpty(tag))
			{
				if (!tags.Items.Contains(tag))
					tags.Items.Add(tag, true);
			}
		}

		private void AddSender(string sender)
		{
			if (!String.IsNullOrEmpty(sender))
			{
				if (!senders.Items.Contains(sender))
					senders.Items.Add(sender, true);
			}
		}

		private void LogTreeDrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			Brush whiteBrush = new SolidBrush(Color.White);
			Brush grayBrush = new SolidBrush(Color.SeaShell);
			int modifier = 0;
			
			if (e.Node.Parent != null)
				modifier = (e.Node.Parent.Index + 1)%2;
			
			if ((e.Node.Index + modifier)%2 == 0)
			{
				e.Graphics.FillRectangle(whiteBrush, e.Bounds);
				e.Node.BackColor = Color.White;
			}
			else
			{
				e.Graphics.FillRectangle(grayBrush, e.Bounds);
				e.Node.BackColor = Color.SeaShell;
			}
			
			e.DrawDefault = true;
		}

		private void CloseMenuClick(object sender, System.EventArgs e)
		{
			Close();
		}

		private void OpenMenuClick(object sender, System.EventArgs e)
		{
			if (openFileDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
				viewModel.File = openFileDialog.FileName;
		}

		private void TypeInformationsCheckedChanged(object sender, EventArgs e)
		{
			viewModel.IncludeInformations = typeInformations.Checked;
		}

		private void TypeWarningsCheckedChanged(object sender, EventArgs e)
		{
			viewModel.IncludeWarnings = typeWarnings.Checked;
		}

		private void TypeErrorsCheckedChanged(object sender, EventArgs e)
		{
			viewModel.IncludeErrors = typeErrors.Checked;
		}

		private void TagsItemCheck(object sender, ItemCheckEventArgs e)
		{
			ICollection<string> selected = viewModel.SelectedTags;
			
			if (!viewModel.IsLoading)
			{
				if (e.NewValue.Equals(CheckState.Checked) || e.NewValue.Equals(CheckState.Unchecked))
				{
					if (e.NewValue.Equals(CheckState.Checked))
						selected.Add(tags.Items[e.Index].ToString());
					if (e.NewValue.Equals(CheckState.Unchecked))
						selected.Remove(tags.Items[e.Index].ToString());
					
					viewModel.SelectedTags = selected;
				}
			}
		}

		private void SendersItemCheck(object sender, ItemCheckEventArgs e)
		{
			ICollection<string> selected = viewModel.SelectedSenders;
			
			if (!viewModel.IsLoading)
			{
				if (e.NewValue.Equals(CheckState.Checked) || e.NewValue.Equals(CheckState.Unchecked))
				{
					if (e.NewValue.Equals(CheckState.Checked))
						selected.Add(senders.Items[e.Index].ToString());
					if (e.NewValue.Equals(CheckState.Unchecked))
						selected.Remove(senders.Items[e.Index].ToString());
					
					viewModel.SelectedSenders = selected;
				}
			}
		}

		private void CopyMessageMenuClick(object sender, EventArgs e)
		{
			if (logTree.SelectedNode != null)
				Clipboard.SetText((logTree.SelectedNode.Tag as LogEntry).Message);
		}

		private void CopyAllMenuClick(object sender, EventArgs e)
		{
			if (logTree.SelectedNode != null)
				Clipboard.SetText((logTree.SelectedNode.Tag as LogEntry).ToString());
		}

		private void LogTreeAfterSelect(object sender, TreeViewEventArgs e)
		{
			copyAllMenu.Enabled = (logTree.SelectedNode != null) && (logTree.SelectedNode.Tag != null);
			copyMessageMenu.Enabled = (logTree.SelectedNode != null) && (logTree.SelectedNode.Tag != null);
		}
		
		public MainForm(MainWindowViewModel viewModel)
		{
			if (viewModel == null)
				throw new ArgumentNullException("viewModel");
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			openFileDialog.InitialDirectory = Environment.CurrentDirectory;

			viewModel.FileChanged += new EventHandler(FileChangedHandler);
			viewModel.FilteredEntriesChanged += new EventHandler(FilteredEntriesChangedHandler);
			viewModel.FilteredSendersChanged += new EventHandler(FilteredSendersChangedHandler);
			viewModel.FilteredTagsChanged += new EventHandler(FilteredTagsChangedHandler);
			viewModel.LoadProgress += new EventHandler<ProgressEventArgs>(ProgressHandler);
			
			this.progressForm = new ProgressForm();
			this.progressForm.Owner = this;
			this.viewModel = viewModel;
		}

		private void ProgressHandler(object sender, ProgressEventArgs e)
		{	
			progressForm.ProgressHadler(sender, e);
		}
	}
}
