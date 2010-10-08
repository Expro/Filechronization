// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    #endregion

    /// <summary>
    /// A relative path. In most cases represents path relative to MainStoragePath in given network.
    /// </summary>
    [Serializable]
    public struct RelPath : IRelPath
    {
        private readonly string _relativePath;

        public RelPath(string relativePath)
        {
            if (relativePath == null || Path.IsPathRooted(relativePath))
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

//        public static implicit operator string(RelPath path)
//        {
//            return path.Get;
//        }

        public static RelPath FromNames(IEnumerable<Name> args)
        {
//            return Path.Combine(args);

            StringBuilder sBuilder = new StringBuilder();
            foreach (Name name in args)
            {
                sBuilder.Append(name + PathUtils.Slash);
            }
            sBuilder.Remove(sBuilder.Length - 1, 1);
            return (RelPath) sBuilder.ToString();
        }



        public static explicit operator RelPath(string relPath)
        {
            return new RelPath(relPath);
        }

        public override string ToString()
        {
            return Get;
        }

        public bool Equals(RelPath other)
        {
            return Equals(other._relativePath, _relativePath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (RelPath)) return false;
            return Equals((RelPath) obj);
        }

        public override int GetHashCode()
        {
            return (_relativePath != null ? _relativePath.GetHashCode() : 0);
        }
    }
}