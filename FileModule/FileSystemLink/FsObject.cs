namespace FileModule
{
    using System;
    using System.IO;
    [Serializable]
    public abstract class FsObject<TPath>where TPath:IPath
    {
        private TPath _path;
        private bool _synchronized;

        protected FsObject(TPath path)
        {
            _path = path;
        }

        public TPath Path
        {
            get { return _path; }
        }

        public bool IsSynchronized
        {
            get { return _synchronized; }
            set { _synchronized = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <returns></returns>
        public static FsObject<AbsPath> NewLocal(AbsPath fullPath)
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