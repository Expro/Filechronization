namespace FileModule
{


    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;

    #endregion

    
    [Serializable]
    public class FileTable
    {
        private readonly MainStoragePath folderPath;


        private readonly Dictionary<AbsPath, FsObject<AbsPath>> table;


        public FileTable(MainStoragePath folderPath)
        {
            this.folderPath = folderPath;
            table = new Dictionary<AbsPath, FsObject<AbsPath>>();
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
                AddAll(folderPath.ToFull((RelPath) fol));
            }
        }


        private void AddAll(AbsPath dir)
        {
            string[] files = Directory.GetFiles(dir, "*");

            foreach (AbsPath s in files)
            {
                AddFile(s);
            }

            string[] dirs = Directory.GetDirectories(dir, "*");
            foreach (AbsPath s in dirs)
            {
                AddFile(s);
                AddAll(s);
            }
        }

        private void AddFile(AbsPath path)
        {
            try
            {
                //var rel = folderPath.CreateRelative(path);
                table.Add(path, FsObject<AbsPath>.NewLocal(path));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void AddFile(FsObject<AbsPath> descriptor)
        {

            table.Add(descriptor.Path, descriptor);

        }
        public FsObject<AbsPath> GetFile(AbsPath absPath)
        {
            return table[absPath];
        }

//        public FileDescriptor FindEqualFile(FileDescriptor pattern)
//        {
//            return table.FirstOrDefault(fileDescriptor => fileDescriptor.Equals(pattern));
//        }
    }
}