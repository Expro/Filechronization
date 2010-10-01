// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using CodeExecutionTools.Logging;

    #endregion

    public class DirNode
    {
        private Name _name;
        private DirNode _parent;
        private Dictionary<Name, DirNode> _directories;
        private Dictionary<Name, FsFile<Name>> _files;

        public DirNode(Name name, DirNode parent)
        {
            _name = name;
            _parent = parent;
            _directories=new Dictionary<Name, DirNode>();
            _files=new Dictionary<Name, FsFile<Name>>();
        }

        public Name Name
        {
            get { return _name; }
        }

        public Dictionary<Name, DirNode> Directories
        {
            get { return _directories; }
        }

        public Dictionary<Name, FsFile<Name>> Files
        {
            get { return _files; }
        }

        public DirNode Parent
        {
            get { return _parent; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns>Created DirNode if parameter is a directory, null otherwise.</returns>
        public DirNode Add(FsObject<AbsPath> descriptor)
        {
            var name = descriptor.Name;
            if (descriptor is FsFile<AbsPath>)
            {
                FsFile<AbsPath> file = (FsFile<AbsPath>)descriptor;
                _files.Add(name, new FsFile<Name>(name, file.Size, file.LastWrite));
                return null;
            }
            else
            {
                var dir = new DirNode(name, this);
                _directories.Add(name, dir);
                return dir;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns>True if parameter is a directory, false otherwise.</returns>
        public bool Remove(FsObject<AbsPath> descriptor)
        {
            var name = descriptor.Name;
            if (descriptor is FsFile<AbsPath>)
            {
                _files.Remove(name);
                return false;
            }
            else
            {
                _directories.Remove(name);
                return true;
            }
        }


        public void GetIndexed(AbsPath currentPath, Dictionary<AbsPath, FsObject<AbsPath>> result)
        {
            foreach (var file in _files.Values)
            {
                AbsPath fullPath = (AbsPath) Path.Combine(currentPath, file.Name);
                result.Add(fullPath, new FsFile<AbsPath>(fullPath, file.Size, file.LastWrite));
            }

            foreach (var dir in _directories.Values)
            {
                AbsPath childPath = (AbsPath) Path.Combine(currentPath, dir.Name);
                result.Add(childPath, new FsFolder<AbsPath>(childPath));
                dir.GetIndexed(childPath, result);
            }
        }


    }


    [Serializable]
    public class FileTable
    {
        private readonly MainStoragePath _mainPath;


        private readonly Dictionary<AbsPath, FsObject<AbsPath>> _table;

        private readonly Dictionary<AbsPath, DirNode> _indexedDirectories;

        private DirNode _rootDirectory;

        public Dictionary<AbsPath, FsObject<AbsPath>> Table
        {
            get { return _table; }
        }





        public FileTable(MainStoragePath mainPath)
        {
            _mainPath = mainPath;
            _table = new Dictionary<AbsPath, FsObject<AbsPath>>();
            _rootDirectory = new DirNode((Name) Path.GetFileName(mainPath.ToString()),null);
            _indexedDirectories = new Dictionary<AbsPath, DirNode>();
        }
        public Dictionary<AbsPath, FsObject<AbsPath>> GetIndexedFor(AbsPath folderPath)
        {
            Dictionary<AbsPath, FsObject<AbsPath>> result = new Dictionary<AbsPath, FsObject<AbsPath>>();

            var parent = _indexedDirectories[folderPath];
            parent.GetIndexed(folderPath, result);
            return result;
        }
//        private void GetAll(DirNode current, string currentPath, Dictionary<AbsPath, FsObject<AbsPath>> result)
//        {
//            string[] files = Directory.GetFiles(dir, "*");
//
//            foreach (var file in current.Files)
//            {
//                result.Add(Path.Combine(currentPath, file.));
//            }
//
//            string[] dirs = Directory.GetDirectories(dir, "*");
//            foreach (AbsPath s in dirs)
//            {
//                AddFile(s);
//                AddAll(s);
//            }
//        }
//        public void AddFile()
//        {
//            if (Path.IsPathRooted(relativePath))
//            {
//                throw new ArgumentException("Path must be relative");
//            }
//        }

        public void AddFolders(IEnumerable<string> subfoldersNames)
        {
            foreach (string fol in subfoldersNames)
            {
                var path = _mainPath.ToFull((RelPath) fol);
                AddFile(path);
                AddAll(path);
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
            FsObject<AbsPath> obj;
            try
            {
                obj = FsObject<AbsPath>.NewLocal(path);
            }
            catch (IOException e) // Might be a bbug.
            {
                LoggingService.Trace.Warning(e.ToString(), sender: this);
                return;
            }
            AddFile(obj);
        }

        public void AddFile(FsObject<AbsPath> descriptor)
        {
            
            _table.Add(descriptor.Path, descriptor);

            DirNode parent = GetParentDir(descriptor);

            var dir = parent.Add(descriptor);

            if(dir!=null)
            {
                _indexedDirectories.Add(descriptor.Path, dir);
            }
            
        }
        private DirNode GetParentDir(FsObject<AbsPath> descriptor)
        {
            var folders = _mainPath.GetParentFolders(descriptor.Path);

            DirNode current = _rootDirectory;
            foreach (Name folderName in folders)
            {
                current = current.Directories[folderName];
            }
            return current;
        }
        public FsObject<AbsPath> GetFile(AbsPath absPath)
        {
            return _table[absPath];
        }
        
//        public FileDescriptor FindEqualFile(FileDescriptor pattern)
//        {
//            return table.FirstOrDefault(fileDescriptor => fileDescriptor.Equals(pattern));
//        }
        public void Remove(FsObject<AbsPath> descriptor)
        {
            _table.Remove(descriptor.Path);
            DirNode parent = GetParentDir(descriptor);
            
            if (parent.Remove(descriptor))
            {
                _indexedDirectories.Remove(descriptor.Path);
            }
        }

      
    }
}