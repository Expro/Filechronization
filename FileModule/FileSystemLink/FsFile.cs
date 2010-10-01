// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.IO;

    #endregion

    [Serializable]
    public class FsFile<TPath> : FsObject<TPath> where TPath : IPath
    {
        private readonly DateTime _lastWrite;
        private readonly long _size;
        private FileVersion _version;

        public FsFile(TPath path, long size, DateTime lastWrite)
            : base(path)
        {
            _size = size;
            _lastWrite = lastWrite;
        }

        public FileVersion Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public long Size
        {
            get { return _size; }
        }

        public DateTime LastWrite
        {
            get { return _lastWrite; }
        }
        /// <summary>
        /// Gets file properies from the filesystem
        /// </summary>
        /// <param name="fullPath">Path to a file.</param>
        /// <returns>Created FsFile></returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static FsFile<AbsPath> LoadFrom(AbsPath fullPath)
        {
            FileInfo info = new FileInfo(fullPath);
            DateTime writeDate = info.LastWriteTimeUtc;
            long len = info.Length;
            return new FsFile<AbsPath>(fullPath, len, writeDate);
        }

        public bool Equals(FsFile<TPath> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return other.Path.Equals(Path) && other.Size == Size && other.LastWrite.Equals(LastWrite);
        }
        /// <summary>
        /// Fast way of determining whether two files are equal without checksums
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool FastEqualityCheck(FsFile<TPath> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return other.Size == Size && other.LastWrite.Equals(LastWrite);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof (FsFile<TPath>))
                return false;
            return Equals((FsFile<TPath>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Size.GetHashCode()*397) ^ LastWrite.GetHashCode();
            }
        }
    }
}