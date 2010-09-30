// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;

    #endregion

    #region Usings

    #endregion

    /// <summary>
    ///   Each group has one or more subfolders
    ///   Each subfolder is bound to only one group
    ///   One group (=) One FileTable
    /// </summary>
    public class TableOverseer : NetworkContextModule
    {
        /// <summary>
        ///   nazwie podfolderu przyporzadkowuje tablice w ktorej sie on znajduje
        /// </summary>
        private readonly Dictionary<string /*Folder name*/, FileTable> subfolderShortcuts;

        /// <summary>
        ///   Kazda grupa zarzadza jedn¹ tablic¹ plikow
        /// </summary>
        private readonly Dictionary<GroupModel, FileTable> tableList;


        public TableOverseer(NetworkContext network)
            : base(network)
        {
            subfolderShortcuts = new Dictionary<string, FileTable>();
            tableList = new Dictionary<GroupModel, FileTable>();
            foreach (GroupModel group in network.GroupList)
            {
                FileTable table = new FileTable(WorkPath);
                tableList.Add(group, table);
                foreach (string folder in group.FolderList)
                {
                    subfolderShortcuts.Add(folder, table);
                }
            }
        }

        /// <summary>
        ///   Searches indexed file table for an object with given path
        /// </summary>
        /// <param name = "fullPath">Path of the object</param>
        /// <returns>Object descriptor</returns>
        /// <exception cref = "System.IO.FileNotFoundException"></exception>
        public FsObject<AbsPath> GetObject(AbsPath fullPath)
        {
            FileTable table = ChooseTable(fullPath);

            //string relativePath = network.MainPath.CreateRelative(fullPath);

            try
            {
                return table.GetFile(fullPath);
            }
            catch (KeyNotFoundException)
            {
                throw new FileNotFoundException("File: " + fullPath + "was not found.");
            }
        }

//        public void IndexFiles(string folderPath)
//        {
//            var table = ChooseTable(folderPath);
//            table.AddFolders(new []{folderPath});
//
//            
//        }

        public void IndexAllFiles()
        {
            foreach (KeyValuePair<GroupModel, FileTable> pair in tableList)
            {
                pair.Value.AddFolders(pair.Key.FolderList);
            }
        }

        /// <summary>
        ///   Selects direct subfolder of main path from path
        /// </summary>
        /// <param name = "path">Path to file in subfolder</param>
        /// <returns></returns>
        public FileTable ChooseTable(IPath path)
        {
            string subfolder = WorkPath.ExtractSubfolderName(path);
            return subfolderShortcuts[subfolder];
            //   subfolderShortcuts.Add();
        }

        public void AddFile(FsObject<AbsPath> descriptor)
        {
            FileTable table = ChooseTable(descriptor.Path);

            table.AddFile(descriptor);
        }

//        public bool CreateIndexingJob(AbsPath folderPath)
//        {
//            var table = ChooseTable(folderPath);
//            table.AddFolders(new []{folderPath});
//        }
        public void RunIndexingJob(IndexingJob indexing, Action<IndexingJob, bool, object> callback, object userState)
        {
            throw new NotImplementedException();
        }

        public void Remove(AbsPath path)
        {
            FileTable table = ChooseTable(path);
            table.Remove(path);
        }
    }
}