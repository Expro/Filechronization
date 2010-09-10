#region Header

// Author: Piotr 'Gibz' Trzpil

#endregion

namespace FileModule
{
    #region Usings

    using System;
    using System.IO;

    #endregion

    [Serializable]
    public class FileDescriptor : FileSystemObjectDescriptor
    {
        private readonly long size;
        private DateTime lastWriteTime;

//        public FileDescriptor(string relativePath, DateTime lastWriteTime)
//            : base(path)
//        {
//            lastWriteTime = File.GetLastWriteTimeUtc(path);
//            var info = new FileInfo(path);
//            size = info.Length;
//        }
        public FileDescriptor(string relativePath, long size, DateTime lastWriteTime)
            : base(relativePath)
        {
            this.size = size;
            this.lastWriteTime = lastWriteTime;
        }


        public static FileDescriptor LoadFrom(MainStoragePath mainPath, string fullPath)
        {
            DateTime writeDate = File.GetLastWriteTimeUtc(fullPath);
            var info = new FileInfo(fullPath);
            long len = info.Length;
            return new FileDescriptor(mainPath.CreateRelative(fullPath), len, writeDate);
        }





        public long Size
        {
            get { return size; }
        }

        public DateTime LastWriteTime
        {
            get { return lastWriteTime; }
        }

        public bool Equals(FileDescriptor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.size == size && other.lastWriteTime.Equals(lastWriteTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FileDescriptor)) return false;
            return Equals((FileDescriptor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (size.GetHashCode()*397) ^ lastWriteTime.GetHashCode();
            }
        }
    }
}