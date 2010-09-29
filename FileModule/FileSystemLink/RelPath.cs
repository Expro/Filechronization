// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.IO;

    #endregion

    [Serializable]
    public struct RelPath : IPath
    {
        private readonly string _relativePath;

        public RelPath(string relativePath)
        {
            if (String.IsNullOrEmpty(relativePath) || Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException("Path " + relativePath + " is not relative.");
            }
            _relativePath = relativePath;
        }

        #region IPath Members

        public string Get
        {
            get { return _relativePath; }
        }

        #endregion

        public static implicit operator string(RelPath path)
        {
            return path.Get;
        }

        public static explicit operator RelPath(string absPath)
        {
            return new RelPath(absPath);
        }

        public override string ToString()
        {
            return Get;
        }
    }
}