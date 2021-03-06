﻿// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.IO;

    #endregion

    [Serializable]
    public abstract class FsObject<TPath> where TPath : IPath
    {
        private readonly TPath _path;
        private bool _synchronized;

        protected FsObject(TPath path)
        {
            _path = path;
        }

        public TPath Path
        {
            get { return _path; }
        }
//        public Name Name
//        {
//            get
//            {
//                throw new NotImplementedException("Do not use.");
                //return (Name) System.IO.Path.GetFileName(Path.ToString());
//            }
//        }
        
        public bool IsSynchronized
        {
            get { return _synchronized; }
            set { _synchronized = value; }
        }
        public bool FastEqualityCheck(FsObject<TPath> other)
        {
            if(GetType() != other.GetType())
            {
                return false;
            }
            var file = other as FsFile<TPath>;
            return file == null || file.FastEqualityCheck((FsFile<TPath>) this);
        }
        
        /// <summary>
        /// Creates new FsObject
        /// </summary>
        /// <param name = "fullPath">Given path</param>
        /// <exception cref = "System.IO.FileNotFoundException"></exception>
        /// <exception cref = "System.IO.IOException"></exception>
        /// <returns>New FsObject</returns>
        public static FsObject<AbsPath> ReadFrom(AbsPath fullPath)
        {
            if (File.Exists(fullPath))
            {
                return FsFile<AbsPath>.LoadFrom(fullPath);
            }
            if (Directory.Exists(fullPath))
            {
                return new FsFolder<AbsPath>(fullPath);
            }
            throw new FileNotFoundException(fullPath);
        }
    }
}