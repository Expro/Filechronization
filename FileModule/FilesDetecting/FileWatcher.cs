// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using CodeExecutionTools.Logging;

    #endregion

    #region Usings

    #endregion

    public class FileWatcher : NetworkContextModule
    {
        #region Delegates

        public delegate void FileChangedEventHandler(FsFile<AbsPath> fileObj);

        public delegate void FileReplacedEventHandler(FsFile<AbsPath> sourceObj, FsFile<AbsPath> targetObj);

        public delegate void ObjectCreatedEventHandler(FsObject<AbsPath> fileObj);

        public delegate void ObjectDeletedEventHandler(AbsPath path);

        public delegate void ObjectPathChangedEventHandler(FsObject<AbsPath> sourceObj, FsObject<AbsPath> targetObj);

        #endregion

        private const long DeleteWaitTime = 500;

        private readonly Dictionary<AbsPath, IndexingJob> _activeIndexings;

        private readonly Dictionary<AbsPath, ObjectDeletedEvent> _deletedFolders;
        private readonly Dictionary<AbsPath, ObjectDeletedEvent> dict;

        //private readonly string folderPath;
        //private FileTable fileTable;

        private readonly QueingThread queue;
        private readonly FileSystemWatcher watcher;
        //private Dictionary<string, IndexingJob> _pendingIndexings;

        private HashSet<AbsPath> _interruptedIndexings;

        public FileWatcher(NetworkContext netContext)
            : base(netContext)
        {
            dict = new Dictionary<AbsPath, ObjectDeletedEvent>();
            // folderPath = path;
            watcher = new FileSystemWatcher(WorkPath.Get);

            watcher.IncludeSubdirectories = true;

            watcher.NotifyFilter = NotifyFilters.LastWrite |
                                   NotifyFilters.FileName | NotifyFilters.DirectoryName;


            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;


            queue = new QueingThread();
            queue.Start();
        }

        public bool Active
        {
            get { return watcher.EnableRaisingEvents; }
            set { watcher.EnableRaisingEvents = value; }
        }

        public event ObjectPathChangedEventHandler MovedRenamed;
        public event FileReplacedEventHandler Replaced;

        public event ObjectDeletedEventHandler Deleted;
        public event ObjectCreatedEventHandler Created;
        public event FileChangedEventHandler Modified;


        private void FileChanged(AbsPath path)
        {
            //queue.Add(()=> AddEvent(path, WatcherChangeTypes.Changed));
        }


        private ObjectDeletedEvent AddDeleteEvent(FsObject<AbsPath> fsObject)
        {
//            FsObject<AbsPath> descr = IndexedTable.GetObject(path);

            ObjectDeletedEvent events = new ObjectDeletedEvent(fsObject.Path, FileEventsTimerEnded);
            _deletedFolders.Add(fsObject.Path, events);


            return events;
        }


        private void FileEventsTimerEnded(object state)
        {
            queue.Add(() =>
            {
                ObjectDeletedEvent deletion = (ObjectDeletedEvent)state;

                if (deletion.IsActive)
                {
                    deletion.IsActive = false;
                    _deletedFolders.Remove(deletion.Path);
                    Deleted(deletion.Path);
                }     
            });
        }

        private void FileCreated(AbsPath path)
        {
//            queue.Add(delegate
//            {
//                ObjectDeletedEvent newFile = AddEvent(path, WatcherChangeTypes.Created);
//
            // Wyszukiwanie zdarzen usuniecia w celu indentyfikacji zmiany nazwy lub przeniesienia
//                foreach (ObjectDeletedEvent other in dict.Values)
//                {
//                    FsObject<AbsPath> otherFile;
//                    if (newFile != other 
//                        && other.queue.Contains(WatcherChangeTypes.Deleted)
//                        && (otherFile = IndexedTable.GetObject(other.filePath)).Equals(newFile._descr))
//                    {
//                        if (newFile.queue.Contains(WatcherChangeTypes.Deleted))
//                        {
            // to nie powinno sie zdarzyc dla folderu
            // (chyba ze go ktos w tym samym momencie usunie
//                            Replaced((FsFile<AbsPath>)other._descr, (FsFile<AbsPath>)newFile._descr);
//                        }
//                        else
//                        {
            // Dla folderu tego sie nie da
//                            MovedRenamed(otherFile, newFile._descr);
//                        }
//
//                        RemoveFileEvents(other.filePath);
//                        RemoveFileEvents(newFile.filePath);
//
//
//                        return;
//                    }
//                }
//
//
//                
//
//            });
        }


        private void OnChanged(object source, FileSystemEventArgs eArgs)
        {
           
            try
            {
                switch (eArgs.ChangeType)
                {
                    case WatcherChangeTypes.Changed:
                    {
                        if (File.Exists(eArgs.FullPath))
                        {
//                            FileChanged((AbsPath) eArgs.FullPath);
                            Console.Out.WriteLine("FileChanged " + eArgs.FullPath);
                        }
                        // else nic nie rob


                        break;
                    }

                    case WatcherChangeTypes.Created:
                    {
                        // jesli nie istnieje, to albo zostal usuniety, wtedy ok
                        // albo przeniesiony, wtedy create ktore zacznie przeszukiwanie
                        // bedzie mialo na liscie odpowiadajacy delete, ale 
                        // porownianie z nim sie nie uda, bo obiektu nie ma w tabeli
                        // wiec zindeksowane pliki zostana uznane za normalnie sie pojawione
                        queue.Add(() =>
                        {
                            if (File.Exists(eArgs.FullPath))
                            {
                                FileCreated((AbsPath) eArgs.FullPath);
                                //Console.Out.WriteLine("FileCreated " + eArgs.FullPath);
                            }
                            else if (Directory.Exists(eArgs.FullPath))
                            {
                                FolderCreated((AbsPath) eArgs.FullPath);
                                //Console.Out.WriteLine("DirectoryCreated " + eArgs.FullPath);
                            }
                        });


                        break;
                    }


                    case WatcherChangeTypes.Deleted:
                    {
                        queue.Add(() => ObjectDeleted((AbsPath) eArgs.FullPath));
                        // Console.Out.WriteLine("ObjectDeleted " + eArgs.FullPath);

                        break;
                    }

                    default:
                    {
                        Console.WriteLine("Default!");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                LoggingService.Trace.Error(e.ToString(), sender:this);
              
            }


            //  Console.WriteLine("File: " + eArgs.FullPath + " " + eArgs.ChangeType);
        }

        private void IndexingJobCallback(IndexingJob indexing, bool success, object userState)
        {
            if (_activeIndexings.ContainsKey(indexing.Dir) && _activeIndexings[indexing.Dir] == indexing)
            {
                _activeIndexings.Remove(indexing.Dir);
            }

            if (success)
            {
                List<AbsPath> folderList = (List<AbsPath>) userState;
                AbsPath path;
                ObjectDeletedEvent delEvent = null;
                if(TryFindMatch(indexing.Table, folderList, out path))
                {
                    delEvent = _deletedFolders[path];
                }
                if (delEvent!=null && delEvent.IsActive)
                {
                    delEvent.IsActive = false;
              
                          // Przeniesienie
              
                    // przywrocenie timera dla pozostalych
//                    foreach (var del in _deletedFolders.Values.Where(del => !del.Path.Equals(path)))
//                    {
//                        del.IsActive = true;
//                    }
            
                }
                else
                {
                    // Stworzenie nowego
                }
            }
            else
            {
                _interruptedIndexings.Add(indexing.Dir);
            }
        }

        private bool TryFindMatch(Dictionary<AbsPath, FsObject<AbsPath>> table, List<AbsPath> folderList,
                                  out AbsPath path)
        {
            throw new NotImplementedException();
        }

        private void FolderCreated(AbsPath fullPath)
        {
            if (TryResetIndexings(fullPath))
            {
                // nie bawimy sie w takie komplikacje

                // indeksowanie parenta powinno zlapac stworzenie tego
                return;
            }

            List<AbsPath> folderList = new List<AbsPath>();
            foreach(var ev in _deletedFolders.Values)
            {
                ev.IsActive = false;
                folderList.Add(ev.Path);
            }

//            List<AbsPath> folderList = _deletedFolders.Values
//                .Select(delEvent => delEvent.Path).ToList();

            //folderList.AddRange(_interruptedIndexings);

            // dodanie do listy wszystkich potencjalnych folderow);
            // sortowanie aby folder o nazwie takiej samej jak ten folder
            // znalazl sie na poczcatku jako najbardziej prawdopodobny
            string name = Path.GetFileName(fullPath);
            folderList.Sort((one, two) =>
            {
                string n = Path.GetFileName(one);
                if (n != null && n.Equals(name))
                {
                    return -1;
                }
                return 1;
            });

            IndexingJob indexing = new IndexingJob(fullPath);
            _activeIndexings.Add(fullPath, indexing);
            IndexedTable.RunIndexingJob(indexing, IndexingJobCallback, folderList);
        }

        private bool TryResetIndexings(AbsPath fullPath)
        {
            IndexingJob indexing;
            if (_activeIndexings.TryGetValue(fullPath, out indexing))
            {
                indexing.Cancel();
                IndexingJob indexing2 = new IndexingJob(fullPath);
                _activeIndexings.Add(fullPath, indexing2);
                IndexedTable.RunIndexingJob(indexing2, IndexingJobCallback, indexing.UserObject);
                return true;
            }
            return false;
        }


        private void ObjectDeleted(AbsPath path)
        {
            
            FsObject<AbsPath> obj = IndexedTable.GetObject(path);

            AddDeleteEvent(obj);

            if (obj is FsFile<AbsPath>)
            {
                FileDeleted(obj as FsFile<AbsPath>);
            }
            else
            {
                FolderDeleted(obj as FsFolder<AbsPath>);
            }
            
        }

        private void FolderDeleted(FsFolder<AbsPath> fsFolder)
        {
            TryResetIndexings(fsFolder.Path);
        }


        private void FileDeleted(FsFile<AbsPath> fsFile)
        {
            throw new NotImplementedException();
        }


        private void OnRenamed(object source, RenamedEventArgs eArgs)
        {
            try
            {
                FsObject<AbsPath> oldDescr = IndexedTable.GetObject((AbsPath)eArgs.OldFullPath);
                FsObject<AbsPath> newDescr = FsObject<AbsPath>.NewLocal((AbsPath)eArgs.FullPath);

                if (!TryResetIndexings(newDescr.Path))
                {
                    MovedRenamed(oldDescr, newDescr);
                }
            }
            catch (Exception e)
            {
                LoggingService.Trace.Error(e.ToString(), sender: this);
            }
        }

        #region Nested type: ObjectDeletedEvent

        public class ObjectDeletedEvent
        {
            private readonly AbsPath _path;

            public bool IsActive { get; set;}
            private readonly Timer _timer;

            public ObjectDeletedEvent(AbsPath path, TimerCallback callback)
            {
                _path = path;
                _timer = new Timer(callback, this, DeleteWaitTime, Timeout.Infinite);
            }

            public AbsPath Path
            {
                get { return _path; }
            }

            public Timer Timer
            {
                get { return _timer; }
            }
        }

        #endregion
    }
}