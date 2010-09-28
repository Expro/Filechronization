namespace FileModule
{
    using System;
    using System.IO;
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

        public string Get
        {
            get
            {
                return _relativePath;
            }
        }
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