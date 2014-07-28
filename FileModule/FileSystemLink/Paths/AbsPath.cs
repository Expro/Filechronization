// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.IO;

    #endregion

    public struct AbsPath : IAbsPath
    {
        private readonly string _absolutePath;

        public AbsPath(string absolutePath)
        {
            if (String.IsNullOrEmpty(absolutePath) || !Path.IsPathRooted(absolutePath))
            {
                throw new ArgumentException("Path " + absolutePath + " is not absolute.");
            }
            _absolutePath = absolutePath;
        }

        #region IPath Members

        public string Get
        {
            get { return _absolutePath; }
        }

        #endregion

        public static implicit operator string(AbsPath path)
        {
            return path.Get;
        }

        public static explicit operator AbsPath(string absPath)
        {
            return new AbsPath(absPath);
        }

        public override string ToString()
        {
            return Get;
        }

        public bool Equals(AbsPath other)
        {
            return Equals(other._absolutePath, _absolutePath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (AbsPath)) return false;
            return Equals((AbsPath) obj);
        }

        public override int GetHashCode()
        {
            return _absolutePath.GetHashCode();
        }
    }
}