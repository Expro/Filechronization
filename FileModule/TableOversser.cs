namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;

    #endregion

    public class TableOverseer
    {
        private readonly NetworkModel network;


        /// <summary>
        ///   nazwie podfolderu przyporzadkowuje tablice w ktorej sie on znajduje
        /// </summary>
        private readonly Dictionary<string/*Folder*/, FileTable> subfolderShortcuts;

        /// <summary>
        /// Kazda grupa zarzadza jedn¹ tablic¹ plikow
        /// </summary>
        private readonly Dictionary<GroupModel, FileTable> tableList;

       

        public TableOverseer(NetworkModel network)
        {
            this.network = network;
            subfolderShortcuts = new Dictionary<string, FileTable>();
            tableList = new Dictionary<GroupModel, FileTable>();
            foreach (GroupModel group in network.GroupList)
            {
                FileTable table = new FileTable(network.MainPath);
                tableList.Add(group, table);
                foreach (string folder in group.folderList)
                {
                    subfolderShortcuts.Add(folder, table);
                }
            }
        }


        public FileDescriptor GetFile(string fullPath)
        {
         
            FileTable table = ChooseTable(fullPath);

            string relativePath = network.MainPath.CreateRelative(fullPath);
            FileDescriptor descr = table.GetFile(relativePath);


            if (descr == null)
            {
                throw new FileNotFoundException("File: " + relativePath + "was not found.");
            }
            return descr;
        }


        public void IndexAllFiles()
        {
            //KeyValuePair<GroupModel, FileTable>
            foreach (KeyValuePair<GroupModel, FileTable> pair in tableList)
            {
                pair.Value.AddFolders(pair.Key.folderList);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private FileTable ChooseTable(string filePath)
        {
            string subfolder = network.MainPath.ExtractSubfolderName(filePath);
            return subfolderShortcuts[subfolder];
         //   subfolderShortcuts.Add();
        }

        public void AddFile(FileSystemObjectDescriptor descriptor)
        {
            var table = ChooseTable(descriptor.RelativePath);

            table.AddFile(descriptor);


        }
    }
}