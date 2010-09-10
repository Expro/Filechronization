/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-01
 * Time: 18:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using Patterns;

namespace CodeExecutionTools.Logging
{
	public class XMLZipLogFileHandler: IDisposablePattern, ILogHandler
	{
		private string fileName;
		private bool disposed;
		private Stream fileStream;
		private Stream zipStream;
		private DataContractSerializer serializer;
		private bool readMode;

		private void Open()
		{
			if (ReadMode)
			{
				fileStream = new FileStream(FileName, FileMode.Open);
				zipStream = new GZipStream(fileStream, CompressionMode.Decompress);
			}
			else
			{
				fileStream = new FileStream(FileName, FileMode.CreateNew);
				zipStream = new GZipStream(fileStream, CompressionMode.Compress);
			}
		}

		protected virtual void OnProgress(ProgressEventArgs e)
		{
			if (Progress != null)
				Progress(this, e);
		}
		
		public XMLZipLogFileHandler(string fileName, bool readMode)
		{
			Contract.Requires(fileName != null);

			this.readMode = readMode;
			this.fileName = fileName;
			this.serializer = new DataContractSerializer(typeof(LogEntry));

			Open();
		}

		~XMLZipLogFileHandler()
		{
			Dispose(false);
		}

		public void Write(LogEntry entry)
		{
			CheckDisposed();

			if (!ReadMode)
			{
				serializer.WriteObject(zipStream, entry);
				zipStream.Flush();
				fileStream.Flush();
			}
		}

		public ICollection<LogEntry> Read()
		{
			ICollection<LogEntry> entries;
			Stream memory;
			string[] parts;
			UTF8Encoding encoder;
			byte[] buffer;
			string worker;
			XmlReader reader;
			Stream source;
			string processName = "Loading: " + FileName;
			int i;
			//TODO: Posprzatac
			CheckDisposed();
			
			OnProgress(new ProgressEventArgs(processName, "Extracting data", 0, 4));
			entries = new HashSet<LogEntry>();
			memory = new MemoryStream();
			zipStream.CopyTo(memory);
			memory.Seek(0, SeekOrigin.Begin);
			OnProgress(new ProgressEventArgs(processName, "Preparation for encoding", 1, 4));
			buffer = new byte[memory.Length];
			memory.Read(buffer, 0, (int)memory.Length);
			OnProgress(new ProgressEventArgs(processName, "Encoding", 2, 4));
			encoder = new UTF8Encoding();
			parts = encoder.GetString(buffer, 0, buffer.Length).Split(new string[] {"</Entry>"}, StringSplitOptions.RemoveEmptyEntries);
			OnProgress(new ProgressEventArgs(processName, "Encoded", 3, 4));
			
			i = 0;
			processName = "Converting encoded data";
			foreach (string entry in parts)
			{
				OnProgress(new ProgressEventArgs(processName, i.ToString() + " of " + parts.Length.ToString(), i++, parts.Length));
				worker = entry;
				worker += "</Entry>";
				
				source = new MemoryStream();
				buffer = encoder.GetBytes(worker);
				source.Write(buffer, 0, buffer.Length);
				source.Seek(0, SeekOrigin.Begin);
				reader = XmlTextReader.Create(source);
			
				entries.Add((LogEntry)serializer.ReadObject(reader));
				
				source.Close();
				source.Dispose();
			}

			memory.Close();
			memory.Dispose();
			
			return entries;
		}

		public void CheckDisposed()
		{
			if (Disposed)
				throw new ObjectDisposedException(ToString());
		}

		public void Dispose(bool disposeManagedResources)
		{
			if (!Disposed)
			{
				zipStream.Close();
				zipStream.Dispose();
				fileStream.Close();
				fileStream.Dispose();

				disposed = true;
			}
		}

		public void Dispose()
		{
			if (!Disposed)
				Dispose(true);
		}

		public bool Disposed
		{
			get {return disposed;}
		}

		public string FileName
		{
			get {return fileName;}
		}

		public bool ReadMode
		{
			get {return readMode;}
		}
		
		public event EventHandler<ProgressEventArgs> Progress;
	}
}
