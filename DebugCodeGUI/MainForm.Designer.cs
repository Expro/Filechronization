/*
 *
 * User: Expro
 * Date: 2010-07-18
 * Time: 01:31
 * 
 * 
 */
using System;
using System.Windows.Forms;
using CodeManagement;

namespace DebugCodeGUI
{
	partial class mainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (IsHandleCreated)
			{
				Invoke(new MethodInvoker(() =>
												{
													if (disposing)
													{
														if (components != null)
															components.Dispose();
													}
													base.Dispose(disposing);
				                         }));
			}
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.split = new System.Windows.Forms.SplitContainer();
			this.type = new System.Windows.Forms.ComboBox();
			this.list = new System.Windows.Forms.ListBox();
			this.stateBox = new System.Windows.Forms.GroupBox();
			this.initializeButton = new System.Windows.Forms.Button();
			this.endButton = new System.Windows.Forms.Button();
			this.restoreButton = new System.Windows.Forms.Button();
			this.pauseButton = new System.Windows.Forms.Button();
			this.startButton = new System.Windows.Forms.Button();
			this.runButton = new System.Windows.Forms.Button();
			this.unloadButton = new System.Windows.Forms.Button();
			this.destroyButton = new System.Windows.Forms.Button();
			this.createButton = new System.Windows.Forms.Button();
			this.LoadButton = new System.Windows.Forms.Button();
			this.authors = new System.Windows.Forms.ListView();
			this.authorName = new System.Windows.Forms.ColumnHeader();
			this.authorEmail = new System.Windows.Forms.ColumnHeader();
			this.descriptions = new System.Windows.Forms.TextBox();
			this.version = new System.Windows.Forms.TextBox();
			this.name = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonsPannel = new System.Windows.Forms.Panel();
			this.closeButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
			this.split.Panel1.SuspendLayout();
			this.split.Panel2.SuspendLayout();
			this.split.SuspendLayout();
			this.stateBox.SuspendLayout();
			this.buttonsPannel.SuspendLayout();
			this.SuspendLayout();
			// 
			// split
			// 
			this.split.Dock = System.Windows.Forms.DockStyle.Fill;
			this.split.Location = new System.Drawing.Point(0, 0);
			this.split.Name = "split";
			// 
			// split.Panel1
			// 
			this.split.Panel1.Controls.Add(this.type);
			this.split.Panel1.Controls.Add(this.list);
			// 
			// split.Panel2
			// 
			this.split.Panel2.Controls.Add(this.stateBox);
			this.split.Panel2.Controls.Add(this.authors);
			this.split.Panel2.Controls.Add(this.descriptions);
			this.split.Panel2.Controls.Add(this.version);
			this.split.Panel2.Controls.Add(this.name);
			this.split.Panel2.Controls.Add(this.label5);
			this.split.Panel2.Controls.Add(this.label3);
			this.split.Panel2.Controls.Add(this.label2);
			this.split.Panel2.Controls.Add(this.label1);
			this.split.Size = new System.Drawing.Size(766, 494);
			this.split.SplitterDistance = 310;
			this.split.TabIndex = 1;
			// 
			// type
			// 
			this.type.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.type.DisplayMember = "1";
			this.type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.type.FormattingEnabled = true;
			this.type.Items.AddRange(new object[] {
									"Modules",
									"Addins",
									"Components"});
			this.type.Location = new System.Drawing.Point(10, 9);
			this.type.Name = "type";
			this.type.Size = new System.Drawing.Size(290, 21);
			this.type.TabIndex = 2;
			this.type.ValueMember = "1";
			this.type.SelectedIndexChanged += new System.EventHandler(this.TypeSelectedIndexChanged);
			// 
			// list
			// 
			this.list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.list.FormattingEnabled = true;
			this.list.Location = new System.Drawing.Point(10, 32);
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(290, 407);
			this.list.TabIndex = 1;
			this.list.SelectedIndexChanged += new System.EventHandler(this.ListSelectedIndexChanged);
			// 
			// stateBox
			// 
			this.stateBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.stateBox.BackColor = System.Drawing.SystemColors.Control;
			this.stateBox.Controls.Add(this.initializeButton);
			this.stateBox.Controls.Add(this.endButton);
			this.stateBox.Controls.Add(this.restoreButton);
			this.stateBox.Controls.Add(this.pauseButton);
			this.stateBox.Controls.Add(this.startButton);
			this.stateBox.Controls.Add(this.runButton);
			this.stateBox.Controls.Add(this.unloadButton);
			this.stateBox.Controls.Add(this.destroyButton);
			this.stateBox.Controls.Add(this.createButton);
			this.stateBox.Controls.Add(this.LoadButton);
			this.stateBox.Location = new System.Drawing.Point(3, 365);
			this.stateBox.Name = "stateBox";
			this.stateBox.Size = new System.Drawing.Size(437, 74);
			this.stateBox.TabIndex = 12;
			this.stateBox.TabStop = false;
			this.stateBox.Text = "State: Unloaded";
			// 
			// initializeButton
			// 
			this.initializeButton.Enabled = false;
			this.initializeButton.Location = new System.Drawing.Point(356, 19);
			this.initializeButton.Name = "initializeButton";
			this.initializeButton.Size = new System.Drawing.Size(75, 23);
			this.initializeButton.TabIndex = 9;
			this.initializeButton.Text = "Initialize";
			this.initializeButton.UseVisualStyleBackColor = true;
			this.initializeButton.Click += new System.EventHandler(this.InitializeButtonClick);
			// 
			// endButton
			// 
			this.endButton.Enabled = false;
			this.endButton.Location = new System.Drawing.Point(251, 45);
			this.endButton.Name = "endButton";
			this.endButton.Size = new System.Drawing.Size(75, 23);
			this.endButton.TabIndex = 8;
			this.endButton.Text = "End";
			this.endButton.UseVisualStyleBackColor = true;
			this.endButton.Click += new System.EventHandler(this.EndButtonClick);
			// 
			// restoreButton
			// 
			this.restoreButton.Enabled = false;
			this.restoreButton.Location = new System.Drawing.Point(170, 45);
			this.restoreButton.Name = "restoreButton";
			this.restoreButton.Size = new System.Drawing.Size(75, 23);
			this.restoreButton.TabIndex = 7;
			this.restoreButton.Text = "Restore";
			this.restoreButton.UseVisualStyleBackColor = true;
			// 
			// pauseButton
			// 
			this.pauseButton.Enabled = false;
			this.pauseButton.Location = new System.Drawing.Point(89, 45);
			this.pauseButton.Name = "pauseButton";
			this.pauseButton.Size = new System.Drawing.Size(75, 23);
			this.pauseButton.TabIndex = 6;
			this.pauseButton.Text = "Pause";
			this.pauseButton.UseVisualStyleBackColor = true;
			// 
			// startButton
			// 
			this.startButton.Enabled = false;
			this.startButton.Location = new System.Drawing.Point(8, 45);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(75, 23);
			this.startButton.TabIndex = 5;
			this.startButton.Text = "Start";
			this.startButton.UseVisualStyleBackColor = true;
			this.startButton.Click += new System.EventHandler(this.StartButtonClick);
			// 
			// runButton
			// 
			this.runButton.Enabled = false;
			this.runButton.Location = new System.Drawing.Point(356, 45);
			this.runButton.Name = "runButton";
			this.runButton.Size = new System.Drawing.Size(75, 23);
			this.runButton.TabIndex = 4;
			this.runButton.Text = "Run";
			this.runButton.UseVisualStyleBackColor = true;
			this.runButton.Click += new System.EventHandler(this.RunButtonClick);
			// 
			// unloadButton
			// 
			this.unloadButton.Enabled = false;
			this.unloadButton.Location = new System.Drawing.Point(251, 19);
			this.unloadButton.Name = "unloadButton";
			this.unloadButton.Size = new System.Drawing.Size(75, 23);
			this.unloadButton.TabIndex = 3;
			this.unloadButton.Text = "Unload";
			this.unloadButton.UseVisualStyleBackColor = true;
			this.unloadButton.Click += new System.EventHandler(this.UnloadButtonClick);
			// 
			// destroyButton
			// 
			this.destroyButton.Enabled = false;
			this.destroyButton.Location = new System.Drawing.Point(170, 19);
			this.destroyButton.Name = "destroyButton";
			this.destroyButton.Size = new System.Drawing.Size(75, 23);
			this.destroyButton.TabIndex = 2;
			this.destroyButton.Text = "Destroy";
			this.destroyButton.UseVisualStyleBackColor = true;
			this.destroyButton.Click += new System.EventHandler(this.DestroyButtonClick);
			// 
			// createButton
			// 
			this.createButton.Enabled = false;
			this.createButton.Location = new System.Drawing.Point(89, 19);
			this.createButton.Name = "createButton";
			this.createButton.Size = new System.Drawing.Size(75, 23);
			this.createButton.TabIndex = 1;
			this.createButton.Text = "Create";
			this.createButton.UseVisualStyleBackColor = true;
			this.createButton.Click += new System.EventHandler(this.CreateButtonClick);
			// 
			// LoadButton
			// 
			this.LoadButton.Enabled = false;
			this.LoadButton.Location = new System.Drawing.Point(8, 19);
			this.LoadButton.Name = "LoadButton";
			this.LoadButton.Size = new System.Drawing.Size(75, 23);
			this.LoadButton.TabIndex = 0;
			this.LoadButton.Text = "Load";
			this.LoadButton.UseVisualStyleBackColor = true;
			this.LoadButton.Click += new System.EventHandler(this.LoadButtonClick);
			// 
			// authors
			// 
			this.authors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.authors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.authorName,
									this.authorEmail});
			this.authors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.authors.Location = new System.Drawing.Point(3, 78);
			this.authors.Name = "authors";
			this.authors.Size = new System.Drawing.Size(437, 93);
			this.authors.TabIndex = 11;
			this.authors.UseCompatibleStateImageBehavior = false;
			this.authors.View = System.Windows.Forms.View.Details;
			// 
			// authorName
			// 
			this.authorName.Text = "Name";
			this.authorName.Width = 193;
			// 
			// authorEmail
			// 
			this.authorEmail.Text = "Email";
			this.authorEmail.Width = 195;
			// 
			// descriptions
			// 
			this.descriptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.descriptions.Location = new System.Drawing.Point(3, 199);
			this.descriptions.Multiline = true;
			this.descriptions.Name = "descriptions";
			this.descriptions.Size = new System.Drawing.Size(437, 160);
			this.descriptions.TabIndex = 10;
			// 
			// version
			// 
			this.version.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.version.Location = new System.Drawing.Point(92, 35);
			this.version.Name = "version";
			this.version.ReadOnly = true;
			this.version.Size = new System.Drawing.Size(348, 20);
			this.version.TabIndex = 7;
			// 
			// name
			// 
			this.name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.name.Location = new System.Drawing.Point(92, 12);
			this.name.Name = "name";
			this.name.ReadOnly = true;
			this.name.Size = new System.Drawing.Size(348, 20);
			this.name.TabIndex = 6;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(3, 35);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(83, 23);
			this.label5.TabIndex = 4;
			this.label5.Text = "Version:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 65);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(83, 15);
			this.label3.TabIndex = 2;
			this.label3.Text = "Authors:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(3, 183);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Description:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// buttonsPannel
			// 
			this.buttonsPannel.Controls.Add(this.closeButton);
			this.buttonsPannel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonsPannel.Location = new System.Drawing.Point(0, 446);
			this.buttonsPannel.Name = "buttonsPannel";
			this.buttonsPannel.Size = new System.Drawing.Size(766, 48);
			this.buttonsPannel.TabIndex = 2;
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.closeButton.Location = new System.Drawing.Point(681, 13);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 0;
			this.closeButton.Text = "Close";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
			// 
			// mainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(766, 494);
			this.Controls.Add(this.buttonsPannel);
			this.Controls.Add(this.split);
			this.Name = "mainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Code Control";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.split.Panel1.ResumeLayout(false);
			this.split.Panel2.ResumeLayout(false);
			this.split.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
			this.split.ResumeLayout(false);
			this.stateBox.ResumeLayout(false);
			this.buttonsPannel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button endButton;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Button pauseButton;
		private System.Windows.Forms.Button restoreButton;
		private System.Windows.Forms.Button initializeButton;
		private System.Windows.Forms.GroupBox stateBox;
		private System.Windows.Forms.Button LoadButton;
		private System.Windows.Forms.Button createButton;
		private System.Windows.Forms.Button destroyButton;
		private System.Windows.Forms.Button unloadButton;
		private System.Windows.Forms.Button runButton;
		private System.Windows.Forms.ColumnHeader authorEmail;
		private System.Windows.Forms.ColumnHeader authorName;
		private System.Windows.Forms.ListView authors;
		private System.Windows.Forms.TextBox descriptions;
		private System.Windows.Forms.ComboBox type;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Panel buttonsPannel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox name;
		private System.Windows.Forms.TextBox version;
		private System.Windows.Forms.ListBox list;
		private System.Windows.Forms.SplitContainer split;
		
		void ListSelectedIndexChanged(object sender, System.EventArgs e)
		{
			CodeDetails code;
			
			if (list.SelectedIndex >= 0)
			{
				code = (CodeDetails)list.Items[list.SelectedIndex];
				
				name.Text = code.Name;
				version.Text = code.Version.ToString();
				
				authors.Items.Clear();
				authors.BeginUpdate();
				
				foreach (Author author in code.Authors)
					authors.Items.Add(author.Name).SubItems.Add(author.Email);
				
				authors.EndUpdate();
				
				var lines = new string[code.Descriptions.Count];
				code.Descriptions.CopyTo(lines, 0);
				descriptions.Lines = lines;
				
				controller = manager.GetController(code);
				
				RefreshState();
			}
		}
		
		void CloseButtonClick(object sender, System.EventArgs e)
		{
			Close();
		}
		
		void LoadButtonClick(object sender, System.EventArgs e)
		{
			controller.Load();
			RefreshState();
		}
		
		void CreateButtonClick(object sender, System.EventArgs e)
		{
			controller.Create();
			RefreshState();
		}
		
		void DestroyButtonClick(object sender, System.EventArgs e)
		{
			controller.Destroy();
			RefreshState();
		}
		
		void UnloadButtonClick(object sender, System.EventArgs e)
		{
			controller.Unload();
			RefreshState();
		}
		
		void RunButtonClick(object sender, System.EventArgs e)
		{
			
		}
		
		void InitializeButtonClick(object sender, EventArgs e)
		{
			controller.Initialize();
			RefreshState();
		}
	}
}
