// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;

    #endregion

    [Serializable]
    public class FsFolder<TPath> : FsObject<TPath> where TPath : IPath
    {
        public FsFolder(TPath path)
            : base(path)
        {
        }
    }
}