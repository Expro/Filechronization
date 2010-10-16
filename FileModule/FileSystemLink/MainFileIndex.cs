// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    #endregion

    #region Usings

    #endregion

    /// <summary>
    ///   Each group has one or more subfolders
    ///   Each subfolder is bound to only one group
    ///   One group (=) One FileTable
    /// </summary>
    public class MainFileIndex : NetworkContextModule
    {
        /// <summary>
        ///   nazwie podfolderu przyporzadkowuje tablice w ktorej sie on znajduje
        /// </summary>
        private readonly Dictionary<Name /*Folder name*/, FileTable> _subfolderShortcuts;

        /// <summary>
        ///   Kazda grupa zarzadza jedn¹ tablic¹ plikow
        /// </summary>
        private readonly Dictionary<GroupModel, FileTable> tableList;


        public MainFileIndex(NetworkContext network)
            : base(network)
        {
            _subfolderShortcuts = new Dictionary<Name, FileTable>();
            tableList = new Dictionary<GroupModel, FileTable>();
            foreach (GroupModel group in network.GroupList)
            {
                FileTable table = new FileTable(MainPath);
                tableList.Add(group, table);
                foreach (string folder in group.FolderList)
                {
                    _subfolderShortcuts.Add((Name) folder, table);
                }
            }
        }

        /// <summary>
        ///   Searches indexed file table for an object with given path
        /// </summary>
        /// <param name = "fullPath">Path of the object</param>
        /// <returns>Object descriptor</returns>
        /// <exception cref = "System.IO.FileNotFoundException"></exception>
        public FsObject<AbsPath> GetObjectAbs(AbsPath fullPath)
        {

            return GetObject(fullPath.RelativeTo(MainPath)).AbsoluteIn(fullPath);
            
        }
        /// <summary>
        ///   Searches indexed file table for an object with given path
        /// </summary>
        /// <param name = "path">Path of the object</param>
        /// <returns>Object descriptor</returns>
        /// <exception cref = "System.IO.FileNotFoundException"></exception>
        public FsObject<RelPath> GetObject(RelPath path)
        {
            FileTable table = ChooseTable(path);

            try
            {
                return table.GetFile(path);
            }
            catch (KeyNotFoundException)
            {
                throw new FileNotFoundException("File: " + path + "was not found.");
            }
        }
//        public void IndexFiles(string folderPath)
//        {
//            var table = ChooseTable(folderPath);
//            table.AddFolders(new []{folderPath});
//
//            
//        }


        public IndexedObjects GetIndexedFor(RelPath folderPath)
        {
            FileTable table = ChooseTable(folderPath);
            return table.CloneSubTree(folderPath, MainPath);
        }
        public void IndexAllFiles()
        {
            foreach (KeyValuePair<GroupModel, FileTable> pair in tableList)
            {
                foreach (string fol in pair.Key.FolderList)
                {
                    var path = ((RelPath)fol).AbsoluteIn(MainPath);

                    pair.Value.AddAllFromFileSystem(path);

                }
                
            }
        }

        /// <summary>
        ///   Selects direct subfolder of main path from path
        /// </summary>
        /// <param name = "path">Path to file in subfolder</param>
        /// <returns></returns>
        public FileTable ChooseTable(RelPath path)
        {
            Name subfolder = MainPath.ExtractSubfolderName(path);
            return _subfolderShortcuts[subfolder];
            //   subfolderShortcuts.Add();
        }

        public void Add(FsObject<RelPath> descriptor)
        {
            FileTable table = ChooseTable(descriptor.Path);

            table.AddObject(descriptor);
        }

//        public bool CreateIndexingJob(AbsPath folderPath)
//        {
//            var table = ChooseTable(folderPath);
//            table.AddFolders(new []{folderPath});
//        }
        public void RunIndexingJob(IndexingJob indexing, Action<IndexingJob, Exception> callback)
        {
            Task.Factory.StartNew(indexing.StartJob)
                .ContinueWith(prev => callback(indexing, prev.Exception));
        }

        public void Remove(FsObject<RelPath> descriptor)
        {
            FileTable table = ChooseTable(descriptor.Path);
            table.Remove(descriptor);
        }
    }
}