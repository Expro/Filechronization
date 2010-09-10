namespace FileModule
{
    #region Usings

    using System;
    using System.IO;
    using System.Runtime.Serialization;

    #endregion
    [Serializable]
    public abstract class FileSystemObjectDescriptor 
    {
        private readonly string relativePath;


//        protected FileSystemObjectDescriptor(string relattpath)
//        {
//            relativePath = mainPath.CreateRelative(path);
//        }
        protected FileSystemObjectDescriptor(string relativePath)
        {
            this.relativePath = relativePath;
        }

        public string RelativePath
        {
            get { return relativePath; }
        }

        public static FileSystemObjectDescriptor New(MainStoragePath mainPath, string fullPath)
        {
            if (File.Exists(fullPath))
            {
                return FileDescriptor.LoadFrom(mainPath, fullPath);
            }
            if (Directory.Exists(fullPath))
            {
                return new FolderDescriptor(mainPath.CreateRelative(fullPath));
            }
            throw new FileNotFoundException(fullPath);
        }
    }
}