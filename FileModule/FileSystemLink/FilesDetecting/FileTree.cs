namespace FileModule
{

    #region Usings

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CodeExecutionTools.Logging;

    #endregion

    public class FileTree : IFileIndex
    {
        private readonly IPath _relativeTo;
        /// <summary>
        /// Root of this FileTree
        /// </summary>
        private DirNode _rootDirectory;

        public FileTree(IPath relativeTo)
        {
            _relativeTo = relativeTo;
            _rootDirectory = new DirNode(relativeTo.FileName(), null);
        }

        protected IPath RootPath
        {
            get
            {
                return _relativeTo;
            }
          
        }


        public void AddAllFromFileSystem(AbsPath indexRoot)
        {

            Stack<AbsPath> directories = new Stack<AbsPath>();
            Stack<DirNode> nodes = new Stack<DirNode>();

            directories.Push(indexRoot);
            nodes.Push(_rootDirectory);

            while (directories.Count != 0)
            {
                string currentDir = directories.Pop();
                DirNode currentNode = nodes.Pop();

                foreach (AbsPath dir in Directory.GetDirectories(currentDir, "*"))
                {
                    directories.Push(dir);
                    DirNode node = currentNode.EnsureDirNode(dir.FileName());
                    nodes.Push(node);
                }

                foreach (AbsPath path in Directory.GetFiles(currentDir, "*"))
                {
                    FsFile<AbsPath> obj = FsFile<AbsPath>.LoadFrom(path);
                    currentNode.AddFile(obj.NewAsName());

                }
            }


        }
        /// <summary>
        /// RelPath must be relative to rootDirectory
        /// </summary>
        /// <param name="descriptor"></param>
        public void AddFile(FsFile<RelPath> descriptor)
        {

            DirNode parent = GetParentDir(descriptor.Path);

            parent.AddFile(descriptor.NewAsName());

//            if (dir != null)
//            {
//                _indexedDirectories.Add(descriptor.Path, dir);
//            }

        }

        public FsObject<RelPath> GetObject(RelPath path)
        {
            
            var folders = path.GetAncestorFolders();
            DirNode current = GetNode(folders);
            Name fName = path.FileName();
            folders.Add(fName);

            FsFile<Name> fsFile;
            if (current.Files.TryGetValue(fName, out fsFile))
            {
                return fsFile.WithNewPath(RelPath.FromNames(folders));
            }

            return current.Directories[fName].FolderInfo.WithNewPath(RelPath.FromNames(folders));

        }
        public FsFile<RelPath> GetFile(RelPath path)
        {
            
            var folders = path.GetAncestorFolders();
            DirNode parent = GetNode(folders);

            Name fName = path.FileName();
            folders.Add(fName);
            return parent.Files[fName].WithNewPath(RelPath.FromNames(folders));

        }

        public bool TryGetFile(RelPath path, out FsFile<RelPath> file)
        {
            try
            {
                file = GetFile(path);
                return true;
            }
            catch (KeyNotFoundException )
            {
                file = default(FsFile<RelPath>);
                return false;
            }
            
        }

        public bool TryGetObject(RelPath path, out FsObject<RelPath> fsObject)
        {
            throw new NotImplementedException();
        }

        public void MoveDirectory(RelPath sourcePath, RelPath targetPath)
        {
            DirNode parent = GetParentDir(sourcePath);
            DirNode node = parent.RemoveDirNode(sourcePath.FileName());

            DirNode target = EnsureNode(targetPath.GetAncestorFolders());


            target.AddDirNode(targetPath.FileName(), node);
        }

        private DirNode GetParentDir(RelPath dirPath)
        {
            var folders = dirPath.GetAncestorFolders();

            DirNode current = _rootDirectory;
            foreach (Name folderName in folders)
            {
                current = current.Directories[folderName];
            }
            return current;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objPath">Object, whose parent must exist.</param>
        /// <returns></returns>
        private DirNode EnsureParentDir(RelPath objPath)
        {
            var folders = objPath.GetAncestorFolders();

            DirNode current = _rootDirectory;
            foreach (Name folderName in folders)
            {
                current = current.EnsureDirNode(folderName);
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

        private DirNode GetNode(IEnumerable<Name> ancestors)
        {
            DirNode current = _rootDirectory;
            foreach (Name folderName in ancestors)
            {
                current = current.Directories[folderName];
            }
            return current;
        }
        private DirNode EnsureNode(IEnumerable<Name> ancestors)
        {
            DirNode current = _rootDirectory;
            foreach (Name folderName in ancestors)
            {
                current = current.EnsureDirNode(folderName);
            }
            return current;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath">Path relative to rootDirectory</param>
        /// <param name="relativeTo">First part of the path of returned files </param>
        /// <returns></returns>
        public FileTree CloneSubTree(RelPath rootPath, IPath relativeTo)
        {
            var tree = new FileTree(relativeTo);

            Stack<DirNode> targetNodes = new Stack<DirNode>();
            Stack<DirNode> sourceNodes = new Stack<DirNode>();


            DirNode localRoot = GetNode(rootPath.GetAncestorFolders());

            tree._rootDirectory = localRoot.Clone(null);
      
            sourceNodes.Push(localRoot);
            targetNodes.Push(tree._rootDirectory);

            while (sourceNodes.Count != 0)
            {
                DirNode currentSource = sourceNodes.Pop();
                DirNode currentTarget = targetNodes.Pop();

                foreach (DirNode sourceChild in currentSource.Directories.Values)
                {
                    var cloned = sourceChild.Clone(currentTarget);
                    currentTarget.Directories.Add(cloned.Name,cloned);

                    sourceNodes.Push(sourceChild);
                    targetNodes.Push(cloned);
                }
            }
            return tree;
        }







        public class DirNode
        {
            private FsFolder<Name> _folderInfo;
            private DirNode _parent;
            private Dictionary<Name, DirNode> _directories;
            private Dictionary<Name, FsFile<Name>> _files;

            public DirNode(Name name, DirNode parent)
            {
                _folderInfo = new FsFolder<Name>(name);
                _parent = parent;
                _directories = new Dictionary<Name, DirNode>();
                _files = new Dictionary<Name, FsFile<Name>>();
            }
            public DirNode(FsFolder<Name> dirInfo, DirNode parent, Dictionary<Name, FsFile<Name>> files)
            {
                _folderInfo = dirInfo;
                _parent = parent;
                _directories = new Dictionary<Name, DirNode>();
                _files = files;
            }
            public DirNode Clone(DirNode parent)
            {
                return new DirNode(_folderInfo, parent, new Dictionary<Name, FsFile<Name>>(_files));
         
            }
            public FsFolder<Name> FolderInfo
            {
                get { return _folderInfo; }
            }

            public Name Name
            {
                get
                {
                    return _folderInfo.Path;
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
            public DirNode EnsureDirNode(Name name)
            {
                DirNode node;
                if (!_directories.TryGetValue(name, out node))
                {
                    node = new DirNode(name, this);
                    _directories.Add(name, node);
                }
                return node;
            }
            public void AddDirNode(Name newName, DirNode node)
            {
                node._folderInfo = node._folderInfo.WithNewPath(newName);
                node._parent = this;
                _directories.Add(newName, node);
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
            

//            public void GetIndexed(RelPath currentPath, IndexedObjects result)
//            {
//                foreach (var file in _files.Values)
//                {
//                    RelPath path = file.Path.FileName().RelativeIn(currentPath);
//                    result.Index.Add(path, new FsFile<RelPath>(path, file.Size, file.LastWrite));
//                }
//
//                foreach (var dir in _directories.Values)
//                {
//                    RelPath childPath = dir.Name.RelativeIn(currentPath);
//                    result.Index.Add(childPath, new FsFolder<RelPath>(childPath));
//                    dir.GetIndexed(childPath, result);
//                }
//            }


            public DirNode RemoveDirNode(Name dirName)
            {
                DirNode node = _directories[dirName];
                _directories.Remove(dirName);
                return node;
            }

            
        }
    }
}