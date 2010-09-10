namespace FileModule
{
    using System;

    [Serializable]
    public class FolderDescriptor : FileSystemObjectDescriptor
    {
        public FolderDescriptor(string relativePath)
            : base(relativePath)
        {
        }

        
    }
}