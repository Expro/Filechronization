// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using CodeExecutionTools.Logging;

    #endregion

    
    [Serializable]
    public class FileTable
    {
        private readonly MainStoragePath _mainPath;


        private readonly Dictionary<RelPath, FsObject<RelPath>> _table;

        private readonly Dictionary<RelPath, DirNode> _indexedDirectories;

        private DirNode _rootDirectory;

        public Dictionary<RelPath, FsObject<RelPath>> Table
        {
            get { return _table; }
        }





        public FileTable(MainStoragePath mainPath)
        {
            _mainPath = mainPath;
            _table = new Dictionary<RelPath, FsObject<RelPath>>();
            _rootDirectory = new DirNode((Name) Path.GetFileName(mainPath.ToString()),null);
            _indexedDirectories = new Dictionary<RelPath, DirNode>();
        }
        public IndexedObjects GetIndexedFor(RelPath folderPath)
        {
            var result = new IndexedObjects(folderPath);

            var parent = _indexedDirectories[folderPath];
            parent.GetIndexed((RelPath) string.Empty, result);
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
                var path = ((RelPath) fol).AbsoluteIn(_mainPath);
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
                obj = FsObject<AbsPath>.ReadFrom(path);
            }
            catch (IOException e) // Might be a bbug.
            {
                LoggingService.Trace.Warning(e.ToString(), sender: this);
                return;
            }
            AddFile(obj.RelativeTo(_mainPath));
        }

        public void AddFile(FsObject<RelPath> descriptor)
        {
            
            _table.Add(descriptor.Path, descriptor);

            DirNode parent = GetParentDir(descriptor);

            var dir = parent.Add(descriptor);

            if(dir!=null)
            {
                _indexedDirectories.Add(descriptor.Path, dir);
            }
            
        }
        private DirNode GetParentDir(FsObject<RelPath> descriptor)
        {
            var folders = descriptor.Path.GetParentFolders();
            
            DirNode current = _rootDirectory;
            foreach (Name folderName in folders)
            {
                current = current.Directories[folderName];
            }
            return current;
      
        }
        public FsObject<RelPath> GetFile(RelPath absPath)
        {
            return _table[absPath];
        }
        
//        public FileDescriptor FindEqualFile(FileDescriptor pattern)
//        {
//            return table.FirstOrDefault(fileDescriptor => fileDescriptor.Equals(pattern));
//        }
        public void Remove(FsObject<RelPath> descriptor)
        {
            _table.Remove(descriptor.Path);
            DirNode parent = GetParentDir(descriptor);
            
            if (parent.Remove(descriptor))
            {
                _indexedDirectories.Remove(descriptor.Path);
            }
        }

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
                _directories = new Dictionary<Name, DirNode>();
                _files = new Dictionary<Name, FsFile<Name>>();
            }

            public Name Name
            {
                get
                {
                    return _name;
                }
            }

            public Dictionary<Name, DirNode> Directories
            {
                get
                {
                    return _directories;
                }
            }

            public Dictionary<Name, FsFile<Name>> Files
            {
                get
                {
                    return _files;
                }
            }

            public DirNode Parent
            {
                get
                {
                    return _parent;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="descriptor"></param>
            /// <returns>Created DirNode if parameter is a directory, null otherwise.</returns>
            public DirNode Add(FsObject<RelPath> descriptor)
            {
                var name = descriptor.Name;
                if (descriptor is FsFile<RelPath>)
                {
                    FsFile<RelPath> file = (FsFile<RelPath>)descriptor;
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
            public bool Remove(FsObject<RelPath> descriptor)
            {
                var name = descriptor.Name;
                if (descriptor is FsFile<RelPath>)
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


            public void GetIndexed(RelPath currentPath, IndexedObjects result)
            {
                foreach (var file in _files.Values)
                {
                    RelPath path = file.Name.RelativeIn(currentPath);
                    result.Index.Add(path, new FsFile<RelPath>(path, file.Size, file.LastWrite));
                }

                foreach (var dir in _directories.Values)
                {
                    RelPath childPath = dir.Name.RelativeIn(currentPath);
                    result.Index.Add(childPath, new FsFolder<RelPath>(childPath));
                    dir.GetIndexed(childPath, result);
                }
            }


        }


    }
}