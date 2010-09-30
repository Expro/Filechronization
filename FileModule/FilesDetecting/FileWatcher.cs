// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using CodeExecutionTools.Logging;
    using System.Linq;
    #endregion

    #region Usings

    #endregion

    public class FileWatcher : NetworkContextModule
    {
        #region Delegates

        public delegate void FileChangedEventHandler(FsFile<AbsPath> oldFileProps, FsFile<AbsPath> newFileProps);

        public delegate void FileReplacedEventHandler(FsFile<AbsPath> sourceObj, FsFile<AbsPath> targetObj);

        public delegate void ObjectCreatedEventHandler(FsObject<AbsPath> fileObj);

        public delegate void ObjectDeletedEventHandler(FsObject<AbsPath> storedProps);

        public delegate void ObjectPathChangedEventHandler(FsObject<AbsPath> sourceObj, FsObject<AbsPath> targetObj);

        #endregion

        private const long DeleteWaitTime = 500;
        private const long NewFileCheckPeriod = 1000;
        private readonly Dictionary<AbsPath, IndexingJob> _activeIndexings;

        private readonly Dictionary<AbsPath, ObjectDeletedEvent> _deletedFolders;
        private readonly Dictionary<AbsPath, ObjectDeletedEvent> _deletedFiles;

        private readonly Dictionary<AbsPath, NewFileTracking> _trackedNewFiles;
        //private readonly string folderPath;
        //private FileTable fileTable;

        private readonly QueingThread _queue;
        private readonly FileSystemWatcher _watcher;
        //private Dictionary<string, IndexingJob> _pendingIndexings;

        //private HashSet<AbsPath> _interruptedIndexings;

        public FileWatcher(NetworkContext netContext)
            : base(netContext)
        {
            _deletedFolders = new Dictionary<AbsPath, ObjectDeletedEvent>();
            _deletedFiles = new Dictionary<AbsPath, ObjectDeletedEvent>();
            _activeIndexings = new Dictionary<AbsPath, IndexingJob>();
            _trackedNewFiles = new Dictionary<AbsPath, NewFileTracking>();

            _watcher = new FileSystemWatcher(WorkPath.Get);

            _watcher.IncludeSubdirectories = true;

            _watcher.NotifyFilter = NotifyFilters.LastWrite |
                                   NotifyFilters.FileName | NotifyFilters.DirectoryName;


            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Deleted += OnChanged;
            _watcher.Renamed += OnRenamed;


            _queue = new QueingThread();
            _queue.Start();
        }

        public bool Active
        {
            get { return _watcher.EnableRaisingEvents; }
            set { _watcher.EnableRaisingEvents = value; }
        }

        public event ObjectPathChangedEventHandler MovedRenamed;
        public event FileReplacedEventHandler Replaced;

        public event ObjectDeletedEventHandler Deleted;
        public event ObjectCreatedEventHandler Created;
        public event FileChangedEventHandler Modified;


        private void FileChanged(AbsPath path)
        {
            
            if(_trackedNewFiles.ContainsKey(path))
            {
                TrackedFileCheck(_trackedNewFiles[path]);
            }
            else
            {
                FsObject<AbsPath> oldFileProps;
                try
                {
                    oldFileProps = IndexedTable.GetObject(path);
                }
                catch (FileNotFoundException)
                {
                    return;
                }
                Modified((FsFile<AbsPath>)oldFileProps, FsFile<AbsPath>.LoadFrom(path));
            }
        }


        private ObjectDeletedEvent AddDeleteEvent(FsObject<AbsPath> fsObject)
        {
//            FsObject<AbsPath> descr = IndexedTable.GetObject(path);

            ObjectDeletedEvent events = new ObjectDeletedEvent(fsObject, FileEventsTimerEnded);
            
            if (fsObject is FsFile<AbsPath>)
            {
                _deletedFiles.Add(fsObject.Path, events);
            }
            else
            {
                _deletedFolders.Add(fsObject.Path, events);
            }

            return events;
        }


        private void FileEventsTimerEnded(object state)
        {
            _queue.Add(() =>
            {
                ObjectDeletedEvent deletion = (ObjectDeletedEvent) state;

                if (deletion.IsActive)
                {
                    deletion.IsActive = false;

                    if (deletion.StoredProps is FsFile<AbsPath>)
                    {
                        _deletedFiles.Remove(deletion.Path);
                    }
                    else
                    {
                        _deletedFolders.Remove(deletion.Path);
                    }
                    Deleted(deletion.StoredProps);
                }
            });
        }

        private void FileCreated(AbsPath path)
        {
            var fsFile = FsFile<AbsPath>.LoadFrom(path);

            List<ObjectDeletedEvent> deletedFilesList = new List<ObjectDeletedEvent>(_deletedFiles.Values);

            try
            {
                ObjectDeletedEvent deleted = deletedFilesList.First(del => del.StoredProps.FastEqualityCheck(fsFile));
                deleted.IsActive = false; // Bezpieczne, nie byloby juz na liscie gdyby bylo juz wczesniej false.
                MovedRenamed(deleted.StoredProps, fsFile);
            }
            catch (InvalidOperationException) // Matching deletion not found
            {
                TrackNewFile(fsFile);
         //       Created(fsFile);
                
            }
        }

        private void TrackNewFile(FsFile<AbsPath> fsFile)
        {
            
            if(!IsFileBeingUsed(fsFile.Path))
            {
                // Empty file or moved from outside.
                Created(fsFile);
            }
            else
            {
                _trackedNewFiles.Add(fsFile.Path, new NewFileTracking(fsFile,TrackedFileCheck));
                //Console.Out.WriteLine(e);
            }
            
        }
        
        private void TrackedFileCheck(object state)
        {
            var tracking = (NewFileTracking) state;

            if(!tracking.IsStopped && !IsFileBeingUsed(tracking.FileProps.Path))
            {
                tracking.Stop();
                _trackedNewFiles.Remove(tracking.FileProps.Path);
                Created(tracking.FileProps);
            }

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

                // TODO: Czesciowe dopasowanie
                if (TryFindMatch(indexing.Table, folderList, out path))
                {
                    delEvent = _deletedFolders[path];
                }
                if (delEvent != null && delEvent.IsActive)
                {
                    delEvent.IsActive = false;

                    // Przeniesienie
                    MovedRenamed(new FsFolder<AbsPath>(path), new FsFolder<AbsPath>(indexing.Dir));
                    

                    // przywrocenie timera dla pozostalych
//                    foreach (var del in _deletedFolders.Values.Where(del => !del.Path.Equals(path)))
//                    {
//                        del.IsActive = true;
//                    }
                }
                else
                {
                    // Stworzenie nowego
                    foreach (var obj in indexing.Table.Values)
                    {
                        Created(obj);
                    }
                
                }
            }
            else
            {
               // _interruptedIndexings.Add(indexing.Dir);
            }
        }

        private bool TryFindMatch(Dictionary<AbsPath, FsObject<AbsPath>> table, List<AbsPath> folderList,
                                  out AbsPath path)
        {
            throw new NotImplementedException();
        }
        private List<AbsPath> SortedDeletionList(Dictionary<AbsPath, ObjectDeletedEvent> deletedMap, AbsPath firstMatchPath)
        {
            List<AbsPath> folderList = new List<AbsPath>(deletedMap.Keys);
            string name = Path.GetFileName(firstMatchPath);
            // dodanie do listy wszystkich potencjalnych folderow);
            // sortowanie aby folder o nazwie takiej samej jak ten folder
            // znalazl sie na poczcatku jako najbardziej prawdopodobny
            folderList.Sort((one, two) =>
            {
                string n = Path.GetFileName(one);
                if (n != null && n.Equals(name))
                {
                    return -1;
                }
                return 1;
            });
            return folderList;
        }
        private void FolderCreated(AbsPath fullPath)
        {
            

            List<AbsPath> folderList = SortedDeletionList(_deletedFolders, fullPath);

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
            TryResetIndexings(obj.Path);
//            if (obj is FsFile<AbsPath>)
//            {
//                FileDeleted(obj as FsFile<AbsPath>);
//            }
//            else
//            {
//                FolderDeleted(obj as FsFolder<AbsPath>);
//            }
        }

//        private void FolderDeleted(FsFolder<AbsPath> fsFolder)
//        {
//            
//        }
//
//
//        private void FileDeleted(FsFile<AbsPath> fsFile)
//        {
//            throw new NotImplementedException();
//        }


        private void OnRenamed(object source, RenamedEventArgs eArgs)
        {

            try
            {
                FsObject<AbsPath> oldDescr = IndexedTable.GetObject((AbsPath) eArgs.OldFullPath);
                FsObject<AbsPath> newDescr = FsObject<AbsPath>.NewLocal((AbsPath) eArgs.FullPath);

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
                            _queue.Add(() => FileChanged((AbsPath) eArgs.FullPath));
                            
                        //    Console.Out.WriteLine("FileChanged " + eArgs.FullPath);
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
                        AbsPath fullPath = (AbsPath) eArgs.FullPath;
                        _queue.Add(() =>
                        {
                            if (TryResetIndexings(fullPath))
                            {
                                // nie bawimy sie w takie komplikacje

                                // indeksowanie parenta powinno zlapac stworzenie tego
                                return;
                            }
                            if (File.Exists(fullPath))
                            {
                                FileCreated(fullPath);
                                //Console.Out.WriteLine("FileCreated " + eArgs.FullPath);
                            }
                            else if (Directory.Exists(fullPath))
                            {
                                FolderCreated(fullPath);
                                //Console.Out.WriteLine("DirectoryCreated " + eArgs.FullPath);
                            }
                        });


                        break;
                    }


                    case WatcherChangeTypes.Deleted:
                    {
                        _queue.Add(() => ObjectDeleted((AbsPath) eArgs.FullPath));
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
                //Console.Out.WriteLine(e);
                LoggingService.Trace.Error(e.ToString(), sender: this);
            }


            //  Console.WriteLine("File: " + eArgs.FullPath + " " + eArgs.ChangeType);
        }
        private static bool IsFileBeingUsed(AbsPath path)
        {
            try
            {
                // Check if is used by another process
                new FileInfo(path).OpenWrite().Close();
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
        public class NewFileTracking
        {
            private readonly FsObject<AbsPath> _fileProps;
            private readonly Timer _tickTimer;
            private bool _stopped;

            public bool IsStopped
            {
                get { return _stopped; }
            }

            public NewFileTracking(FsObject<AbsPath> fileProps, TimerCallback callback)
            {
                _fileProps = fileProps;
                _tickTimer = new Timer(callback, this, NewFileCheckPeriod, NewFileCheckPeriod);
            }

            public FsObject<AbsPath> FileProps
            {
                get { return _fileProps; }
            }
            public void Stop()
            {
                _stopped = true;
                _tickTimer.Dispose();
            }
        }
        #region Nested type: ObjectDeletedEvent

        public class ObjectDeletedEvent
        {
            private readonly FsObject<AbsPath> _storedProps;
            private readonly Timer _timer;

            public ObjectDeletedEvent(FsObject<AbsPath> storedProps, TimerCallback callback)
            {
                _storedProps = storedProps;
                _timer = new Timer(callback, this, DeleteWaitTime, Timeout.Infinite);
                IsActive = true;
            }

            public bool IsActive { get; set; }

            public FsObject<AbsPath> StoredProps
            {
                get { return _storedProps; }
            }

            public AbsPath Path
            {
                get { return _storedProps.Path; }
            }

            public Timer Timer
            {
                get { return _timer; }
            }
        }

        #endregion
    }
}