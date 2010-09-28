namespace FileModule
{
    using System;

    [Serializable]
    public class FsFolder<TPath> : FsObject<TPath> where TPath : IPath
    {
        public FsFolder(TPath path)
            : base(path)
        {
        }

    }
}