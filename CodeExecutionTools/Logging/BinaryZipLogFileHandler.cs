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
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using Patterns;
//TODO: dodac obsluge zdarzenia Progress
namespace CodeExecutionTools.Logging
{
	public class BinaryZipLogFileHandler: IDisposablePattern, ILogHandler
	{
		private string fileName;
		private bool disposed;
		private Stream fileStream;
		private Stream zipStream;
		private BinaryFormatter formatter;
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

		public BinaryZipLogFileHandler(string fileName, bool readMode)
		{
			Contract.Requires(fileName != null);

			this.readMode = readMode;
			this.fileName = fileName;
			this.formatter = new BinaryFormatter();

			Open();
		}

		~BinaryZipLogFileHandler()
		{
			Dispose(false);
		}

		public void Write(LogEntry entry)
		{
			CheckDisposed();

			if (!ReadMode)
			{
				formatter.Serialize(zipStream, entry);
				zipStream.Flush();
				fileStream.Flush();
			}
		}

		public ICollection<LogEntry> Read()
		{
			ICollection<LogEntry> entries;

			CheckDisposed();

			entries = new HashSet<LogEntry>();

			try
			{
				while (true)
					entries.Add((LogEntry)formatter.Deserialize(zipStream));
			}
			catch (Exception)
			{
				//nothing
			}

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
