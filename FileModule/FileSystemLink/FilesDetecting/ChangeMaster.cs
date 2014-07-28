// Author: Piotr Trzpil

namespace FileModule
{
    #region Usings

    using System;
    using System.IO;

    #endregion

   
    /// <summary>
    /// Handles upper level of detecting filesystem changes:
    /// Is aware of directories belonging to different groups.
    /// Makes changes to MainFileIndex and dispatches commands to ExchangeSystem.
    /// </summary>
    public class ChangeMaster : NetworkContextModule
    {
       

        public ChangeMaster(NetworkContext netContext)
            : base(netContext)
        {
            ChangeWatcher.Created += FileCreated;
            ChangeWatcher.Deleted += FileDeleted;
            ChangeWatcher.Modified += FileModified;
            ChangeWatcher.MovedRenamed += FileMovedRenamed;
            ChangeWatcher.Replaced += FileReplaced;
        }

        private void FileReplaced(FsObject<RelPath> sourceObj, FsObject<RelPath> targetObj)
        {
            Console.WriteLine("File Replaced from: " + sourceObj.Path + " to: " + targetObj.Path);
        }

        private bool SameGroup(RelPath sourcepath, RelPath targetpath)
        {
            string srcDir = MainPath.ExtractSubfolderName(sourcepath);
            string tarDir = MainPath.ExtractSubfolderName(targetpath);

            if (srcDir == tarDir)
            {
                return true;
            }
            if (FileIndex.ChooseTable(sourcepath)
                != FileIndex.ChooseTable(targetpath))
            {
                return false;
            }
            return true;
        }

        private void FileMovedRenamed(FsObject<RelPath> sourceFile, FsObject<RelPath> targetFile)
        {
            if (SameGroup(sourceFile.Path, targetFile.Path))
            {
                Console.WriteLine("File Moved or Renamed from: " + sourceFile.Path + " to: " + targetFile.Path);
                FileIndex.Remove(sourceFile);
                FileIndex.Add(targetFile);
            }
            else
            {
                FileDeleted(sourceFile);
                FileCreated(targetFile);
            }

            
        }

        private void FileModified(FsFile<RelPath> oldFileProps, FsFile<RelPath> newFileProps)
        {
            Console.WriteLine("File Modified: " + newFileProps.Path);
            FileIndex.Remove(oldFileProps);
            FileIndex.Add(newFileProps);
        }

        private void FileDeleted(FsObject<RelPath> storedDescr)
        {
            Console.WriteLine("File Deleted: " + storedDescr.Path);
            FileIndex.Remove(storedDescr);
        }

       
        private void FileCreated(FsObject<RelPath> descriptor)
        {
            
            // string relPath = fileModule.Network.MainPath.CreateRelative(path);
            Console.WriteLine("Object Created: {0}", descriptor.Path);

            FileIndex.Add(descriptor);
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