namespace FileModule
{

    #region Usings

    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CodeExecutionTools.Logging;

    #endregion

    public class FileTree
    {
        private readonly RelPath _relativeTo;
        private DirNode _rootDirectory;

        public FileTree(RelPath relativeTo)
        {
            _relativeTo = relativeTo;
            _rootDirectory = new DirNode(relativeTo.FileName(), null);
        }


        public void AddAll(AbsPath indexRoot, IAbsPath mainPath)
        {
            FileTree tree = new FileTree(indexRoot.RelativeTo(mainPath));


            Stack<AbsPath> directories = new Stack<AbsPath>();
            Stack<DirNode> nodes = new Stack<DirNode>();

            directories.Push(indexRoot);
            nodes.Push(tree._rootDirectory);

            while (directories.Count != 0)
            {
                string currentDir = directories.Pop();
                DirNode currentNode = nodes.Pop();

                foreach (AbsPath dir in Directory.GetDirectories(currentDir, "*"))
                {
                    directories.Push(dir);
                    currentNode.AddDirNode(dir.FileName());
                }

                foreach (AbsPath path in Directory.GetFiles(currentDir, "*"))
                {
                    FsFile<AbsPath> obj = FsFile<AbsPath>.LoadFrom(path);
                    currentNode.AddFile(obj.NewAsName());

                }
            }


        }
        public void AddFile(FsObject<RelPath> descriptor)
        {

            DirNode parent = GetParentDir(descriptor);

            var dir = parent.AddFile(descriptor.);

            if (dir != null)
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
//        private void AddFile(AbsPath path)
//        {
//            FsObject<AbsPath> obj;
//            try
//            {
//                obj = FsObject<AbsPath>.ReadFrom(path);
//            }
//            catch (IOException e) // Might be a bbug.
//            {
//                LoggingService.Trace.Warning(e.ToString(), sender: this);
//                return;
//            }
//            AddFile(obj.RelativeTo(_mainPath));
//        }












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
            public DirNode AddDirNode(Name name)
            {
                DirNode node = new DirNode(name, this);
                _directories.Add(name, node);
                return node;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="descriptor"></param>
            /// <returns>Created DirNode if parameter is a directory, null otherwise.</returns>
//            public DirNode Add(FsObject<RelPath> descriptor)
//            {
//                var name = descriptor.Name;
//                if (descriptor is FsFile<RelPath>)
//                {
//                    FsFile<RelPath> file = (FsFile<RelPath>)descriptor;
//                    _files.Add(name, new FsFile<Name>(name, file.Size, file.LastWrite));
//                    return null;
//                }
//                else
//                {
//                    var dir = new DirNode(name, this);
//                    _directories.Add(name, dir);
//                    return dir;
//                }
//            }
            public void AddFile(FsFile<Name> descriptor)
            {
                _files.Add(descriptor.Path, descriptor);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="descriptor"></param>
            /// <returns>True if parameter is a directory, false otherwise.</returns>
            public bool Remove(FsObject<RelPath> descriptor)
            {
                var name = descriptor.Path.FileName();
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
                    RelPath path = file.Path.FileName().RelativeIn(currentPath);
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