// Author: Piotr Trzpil

namespace FileModule
{
    #region Usings

    using System;
    using System.IO;

    #endregion

    #region Usings

    #endregion

    public class MessageDispatcher
    {
        private readonly NetworkContext _netContext;

        private int counter;

        public MessageDispatcher(NetworkContext netContext)
        {
            _netContext = netContext;


            _netContext.FileWatcher.Created += FileCreated;
            _netContext.FileWatcher.Deleted += FileDeleted;
            _netContext.FileWatcher.Modified += FileModified;
            _netContext.FileWatcher.MovedRenamed += FileMovedRenamed;
            _netContext.FileWatcher.Replaced += FileReplaced;
        }

        private void FileReplaced(FsObject<AbsPath> sourceObj, FsObject<AbsPath> targetObj)
        {
//            string source = _netContext.Path.CreateRelative(sourcepath);
//            string target = _netContext.Path.CreateRelative(targetpath);
            Console.WriteLine("File Replaced from: " + sourceObj.Path + " to: " + targetObj.Path);
        }

        private bool SameGroup(IPath sourcepath, IPath targetpath)
        {
            string srcDir = _netContext.Path.ExtractSubfolderName(sourcepath);
            string tarDir = _netContext.Path.ExtractSubfolderName(targetpath);

            if (srcDir == tarDir)
            {
                return true;
            }
            if (_netContext.TableOverseer.ChooseTable(sourcepath)
                != _netContext.TableOverseer.ChooseTable(targetpath))
            {
                return false;
            }
            return true;
        }

        private void FileMovedRenamed(FsObject<AbsPath> sourceFile, FsObject<AbsPath> targetFile)
        {
            // Jesli zostal przeniesiony do innej grupy - zamienic na delete + create
            //LocalObject sourceFile = (LocalObject) sourceObj;
            if (SameGroup(sourceFile.Path, targetFile.Path))
            {
//                var message = new 
            }
            else
            {
                FileDeleted(sourceFile.Path);
                FileCreated(FsObject<AbsPath>.NewLocal(targetFile.Path));
            }

//            string source = _netContext.Path.CreateRelative(sourcepath);
//            string target = _netContext.Path.CreateRelative(targetpath);

            Console.WriteLine("File Moved or Renamed from: " + sourceFile.Path + " to: " + targetFile.Path);
        }

        private void FileModified(FsObject<AbsPath> newDescriptor)
        {
            Console.WriteLine("File Modified: " + newDescriptor.Path);
        }

        private void FileDeleted(AbsPath absPath)
        {
//            string relPath = _netContext.Path.CreateRelative(path);
            Console.WriteLine("File Deleted: " + absPath);
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

        private void FileCreated(FsObject<AbsPath> descriptor)
        {
            counter++;
            // string relPath = fileModule.Network.MainPath.CreateRelative(path);
            Console.WriteLine("Object Created ({0}): {1}", counter, descriptor.Path);

            _netContext.TableOverseer.AddFile(descriptor);
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