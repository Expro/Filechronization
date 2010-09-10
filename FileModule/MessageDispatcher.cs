namespace FileModule
{
    #region Usings

    using System;
    using System.IO;

    #endregion

    public class MessageDispatcher
    {
        private readonly NewFileModule fileModule;
        private int counter;

        public MessageDispatcher(NewFileModule fileModule)
        {
            this.fileModule = fileModule;

            fileModule.fileWatcher.Created += FileCreated;
            fileModule.fileWatcher.Deleted += FileDeleted;
            fileModule.fileWatcher.Modified += FileModified;
            fileModule.fileWatcher.MovedRenamed += FileMovedRenamed;
            fileModule.fileWatcher.Replaced += FileReplaced;
        }

        private void FileReplaced(string sourcepath, string targetpath)
        {
            string source = fileModule.Network.MainPath.CreateRelative(sourcepath);
            string target = fileModule.Network.MainPath.CreateRelative(targetpath);
            Console.WriteLine("File Replaced from: " + source + " to: " + target);
        }

        private void FileMovedRenamed(string sourcepath, string targetpath)
        {
            string source = fileModule.Network.MainPath.CreateRelative(sourcepath);
            string target = fileModule.Network.MainPath.CreateRelative(targetpath);

            Console.WriteLine("File Moved or Renamed from: " + source + " to: " + target);
        }

        private void FileModified(FileSystemObjectDescriptor newDescriptor)
        {
            
            Console.WriteLine("File Modified: " + newDescriptor.RelativePath);
        }

        private void FileDeleted(string path)
        {
            string relPath = fileModule.Network.MainPath.CreateRelative(path);
            Console.WriteLine("File Deleted: " + relPath);
        }

        public FileOrFolder GetObjectType(string path)
        {
            if (File.Exists(path))
            {
                return FileOrFolder.File;
            }
            if (Directory.Exists(path))
            {
                return FileOrFolder.Folder;
            }

            throw new FileNotFoundException();
        }

        private void FileCreated(FileSystemObjectDescriptor descriptor)
        {
            counter++;
           // string relPath = fileModule.Network.MainPath.CreateRelative(path);
            Console.WriteLine("File Created ({0}): {1}", counter, descriptor.RelativePath);

            fileModule.tableOverseer.AddFile(descriptor);
//            try
//            {
//                FileOrFolder objectType = GetObjectType(path);
//                string relativePath = fileModule.Network.MainPath.CreateRelative(path);
//                var message = new SingleFileMessage(relativePath, objectType, SinglePathFileEvent.Created);
//
//                var task = new FileCreatedFirstSideTask(fileModule, message);
//            }
//            catch (Exception e)
//            {
//                
//                Console.WriteLine(e);
//            }
        }
    }
}