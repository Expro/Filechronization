namespace FileModule
{
    public interface IFileIndex
    {

        void AddFile(FsFile<RelPath> descriptor);
        FsFile<RelPath> GetFile(RelPath path);
        bool TryGetFile(RelPath path, out FsFile<RelPath> file);
    }




}