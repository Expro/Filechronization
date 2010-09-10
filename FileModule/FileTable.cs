namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;

    #endregion

    public class FileTable
    {
        private readonly MainStoragePath folderPath;


        private readonly Dictionary<string, FileSystemObjectDescriptor> table;


        public FileTable(MainStoragePath folderPath)
        {
            this.folderPath = folderPath;
            table = new Dictionary<string, FileSystemObjectDescriptor>();
        }

//        public void AddFile()
//        {
//            if (Path.IsPathRooted(relativePath))
//            {
//                throw new ArgumentException("Path must be relative");
//            }
//        }

        public void AddFolders(IEnumerable<string> subfoldersNames)
        {
            foreach (var fol in subfoldersNames)
            {
                AddAll(folderPath.CreateFullPath(fol));
            }
        }


        private void AddAll(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*");

            foreach (string s in files)
            {
                AddFile(s);
            }

            string[] dirs = Directory.GetDirectories(dir, "*");
            foreach (string s in dirs)
            {
                AddFile(s);
                AddAll(s);
            }
        }

        private void AddFile(string path)
        {
            try
            {
                var rel = folderPath.CreateRelative(path);
                table.Add(rel, FileSystemObjectDescriptor.New(folderPath, path));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void AddFile(FileSystemObjectDescriptor descriptor)
        {

            table.Add(descriptor.RelativePath, descriptor);

        }
        public FileDescriptor GetFile(string relativePath)
        {
            FileSystemObjectDescriptor descr;
            table.TryGetValue(relativePath, out descr);
            return descr as FileDescriptor;
        }

//        public FileDescriptor FindEqualFile(FileDescriptor pattern)
//        {
//            return table.FirstOrDefault(fileDescriptor => fileDescriptor.Equals(pattern));
//        }
    }
}