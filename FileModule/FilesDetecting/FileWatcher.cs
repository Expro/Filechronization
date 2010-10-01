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

        public delegate void TimedActionHandler(FsObject<AbsPath> storedProps);

        public delegate void ObjectPathChangedEventHandler(FsObject<AbsPath> sourceObj, FsObject<AbsPath> targetObj);

        #endregion

        private const long DeleteWaitTime = 500;
        private const long NewFileCheckPeriod = 1000;
        private const long PairedCheckWaitTime = 100;

        private readonly Dictionary<AbsPath, IndexingJob> _activeIndexings;

        private readonly TimedMap<TimedFolderDeletion> _deletedFolders;
        private readonly TimedMap<TimedAction> _deletedFiles;

        private readonly TimedMap<TimedAction> _trackedNewFiles;

        private readonly TimedMap<TimedAction> _pairChangeTimings;
        //private readonly string folderPath;
        //private FileTable fileTable;

        private readonly QueingThread _queue;
        private readonly FileSystemWatcher _watcher;
        //private Dictionary<string, IndexingJob> _pendingIndexings;

        //private HashSet<AbsPath> _interruptedIndexings;

        public FileWatcher(NetworkContext netContext)
            : base(netContext)
        {
            _deletedFolders = new TimedMap<TimedFolderDeletion>();
            _deletedFiles = new TimedMap<TimedAction>();

            _activeIndexings = new Dictionary<AbsPath, IndexingJob>();
            _trackedNewFiles = new TimedMap<TimedAction>();
            _pairChangeTimings = new TimedMap<TimedAction>();

            _watcher = new FileSystemWatcher(WorkPath.ToString());

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
//        public class DeletedFolderProps
//        {
//            private TimedAction _timing;
//            private Dictionary<AbsPath, FsObject<AbsPath>> _indexedContent;
//
//            public DeletedFolderProps(TimedAction timing, Dictionary<AbsPath, FsObject<AbsPath>> indexedContent)
//            {
//                _timing = timing;
//                _indexedContent = indexedContent;
//            }
//
//            public TimedAction Timing
//            {
//                get { return _timing; }
//            }
//
//            public Dictionary<AbsPath, FsObject<AbsPath>> IndexedContent
//            {
//                get { return _indexedContent; }
//            }
//        }
        public bool Active
        {
            get { return _watcher.EnableRaisingEvents; }
            set { _watcher.EnableRaisingEvents = value; }
        }

        public event ObjectPathChangedEventHandler MovedRenamed;
        public event FileReplacedEventHandler Replaced;

        public event TimedActionHandler Deleted;
        public event ObjectCreatedEventHandler Created;
        public event FileChangedEventHandler Modified;


        #region FileEvents

        private void FileChanged(AbsPath path)
        {
            TimedAction timed;
            if (_trackedNewFiles.TryGetValue(path, out timed))
            {
                // File is new, not yet in the table
                TrackedFileCheck(timed);
            }
            else 
            {

                FsFile<AbsPath> oldFileProps;
                FsFile<AbsPath> newFileProps;
//                try
//                {
                oldFileProps = (FsFile<AbsPath>) IndexedTable.GetObject(path);
                newFileProps = FsFile<AbsPath>.LoadFrom(path);
                if(_pairChangeTimings.TryGetValue(path, out timed))
                {
                    RemoveTimedEvent(_pairChangeTimings, timed);
                    Modified(oldFileProps, newFileProps);
                }
                else
                {
                    timed = new TimedAction(newFileProps, FireChanged, PairedCheckWaitTime, Timeout.Infinite);
                    _pairChangeTimings.Add(path, timed);
                    return;
                }
//                }
//                catch (FileNotFoundException)
//                {
//                    return;
//                }
                
            }
            
        }

        private void FireChanged(object state)
        {
            _queue.Add(() =>
            {
                var timing = (TimedAction)state;
                if (!timing.IsStopped)
                {
                    RemoveTimedEvent(_pairChangeTimings, timing);
                    //            _pairChangeTimings.Remove(timing.Path);
                    //            timing.Stop();
                    FsFile<AbsPath> oldFileProps = (FsFile<AbsPath>)IndexedTable.GetObject(timing.Path);
                    Modified(oldFileProps, (FsFile<AbsPath>)timing.Descriptor);
                }
                
            });
            

        }

       

        private void FileCreated(AbsPath path)
        {
            var fsFile = FsFile<AbsPath>.LoadFrom(path);

            List<TimedAction> deletedFilesList = new List<TimedAction>(_deletedFiles.Values);

            try
            {
                TimedAction deleted = deletedFilesList.First(del => del.Descriptor.FastEqualityCheck(fsFile));
                RemoveTimedEvent(_deletedFiles, deleted);// Bezpieczne, nie byloby juz na liscie gdyby bylo juz wczesniej zatrzymane.
                //   deleted.Stop(); 
                MovedRenamed(deleted.Descriptor, fsFile);
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
                var tracking = new TimedAction(fsFile, TrackedFileCheck, NewFileCheckPeriod, NewFileCheckPeriod);
                _trackedNewFiles.Add(fsFile.Path, tracking);
                //Console.Out.WriteLine(e);
            }
            
        }
        
        private void TrackedFileCheck(object state)
        {
            var tracking = (TimedAction) state;

            if(!tracking.IsStopped && !IsFileBeingUsed(tracking.Descriptor.Path))
            {
                RemoveTimedEvent(_trackedNewFiles, tracking);
//                tracking.Stop();
//                _trackedNewFiles.Remove(tracking.Descriptor.Path);
                Created(tracking.Descriptor);
            }

        }

        #endregion

        #region Common: Deleted
        private void ObjectDeleted(AbsPath path)
        {
            FsObject<AbsPath> fsObject = IndexedTable.GetObject(path);

      
            if (fsObject is FsFile<AbsPath>)
            {
                TimedAction events = new TimedAction(fsObject, DeleteTimerEnded, DeleteWaitTime, Timeout.Infinite);
                _deletedFiles.Add(fsObject.Path, events);
            }
            else
            {
                
                var indexed = IndexedTable.GetIndexedFor(fsObject.Path);
                TimedFolderDeletion events = new TimedFolderDeletion(fsObject, DeleteTimerEnded, indexed);
                _deletedFolders.Add(fsObject.Path, events); 
            }
            TryResetIndexings(fsObject.Path);
        }

        private void DeleteTimerEnded(object state)
        {
            _queue.Add(() =>
            {
                TimedAction deletion = (TimedAction)state;

                if (!deletion.IsStopped)
                {
                    if (deletion.Descriptor is FsFile<AbsPath>)
                    {
                        RemoveTimedEvent(_deletedFiles, deletion);
                    }
                    else
                    {
                        RemoveTimedEvent(_deletedFolders, deletion);
                    }
                    Deleted(deletion.Descriptor);
                }
            });
        }
        #endregion

        

        private void IndexingJobCallback(IndexingJob finishedIndexing, Exception exception, object userState)
        {
            _queue.Add(() =>
            {
                IndexingJob indexingJob;
                if (_activeIndexings.TryGetValue(finishedIndexing.Dir, out indexingJob) && indexingJob == finishedIndexing)
                {
                    _activeIndexings.Remove(finishedIndexing.Dir);
                }

                if (exception==null)
                {
                    AbsPath path;
                    TimedAction delEvent = null;
                    List<FsObject<AbsPath>> deleted;
                    List<FsFile<AbsPath>> modified;
                    List<FsObject<AbsPath>> newObjects;
                    // TODO: Czesciowe dopasowanie
                    if (TryFindMatch(finishedIndexing.Table, userState as List<Tuple<AbsPath, Dictionary<AbsPath, FsObject<AbsPath>>>>
                        , out path, out deleted, out newObjects, out modified))
                    {
                        delEvent = _deletedFolders[path];
                    }
                    if (delEvent != null && !delEvent.IsStopped)
                    {

                        RemoveTimedEvent(_deletedFolders, delEvent);

                        MovedRenamed(new FsFolder<AbsPath>(path), new FsFolder<AbsPath>(finishedIndexing.Dir));
                        foreach (FsObject<AbsPath> fsObject in deleted)
                        {
                            Deleted(fsObject);
                        }
                        foreach (FsFile<AbsPath> fsObject in modified)
                        {
                            Modified((FsFile<AbsPath>)IndexedTable.GetObject(fsObject.Path), fsObject);

                        }
                        foreach (FsObject<AbsPath> newObject in newObjects)
                        {
                            Created(newObject);
                        }
                    }
                    else
                    {
                        Created(new FsFolder<AbsPath>(finishedIndexing.Dir));
                        // Stworzenie nowego
                        foreach (var obj in finishedIndexing.Table.Values)
                        {
                            Created(obj);
                        }

                    }
                }
                else
                {
                    LoggingService.Trace.Error(exception.ToString());
                }
            });
            

        }

        #region MatchFunction
        //TODO: Optymalizacja

        /// <summary>
        /// Finds a folder whose content matches given indexed object tree.
        /// </summary>
        /// <param name="indexedObjects">Newly indexed objects</param>
        /// <param name="deletedFoldersToCheck">List of pairs of folder path and indexed folder content as Dictionary</param>
        /// <param name="resultPath">Path to matching folder</param>
        /// <param name="deleted"></param>
        /// <param name="newObjects"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        private static bool TryFindMatch(Dictionary<AbsPath, FsObject<AbsPath>> indexedObjects,
            List<Tuple<AbsPath, Dictionary<AbsPath, FsObject<AbsPath>>>> deletedFoldersToCheck,
                                  out AbsPath resultPath, out List<FsObject<AbsPath>> deleted, 
            out List<FsObject<AbsPath>> newObjects, out List<FsFile<AbsPath>> modified)
        {
            resultPath = default(AbsPath);
            deleted = default(List<FsObject<AbsPath>>);
            newObjects = default(List<FsObject<AbsPath>>);
            modified = default(List<FsFile<AbsPath>>);
            if (deletedFoldersToCheck.Count == 0)
            {
                return false;
            }
//            List<FsObject<AbsPath>> deleted = new List<FsObject<AbsPath>>();
//            List<FsObject<AbsPath>> newAndModified = new List<FsObject<AbsPath>>();

            foreach (var pair in deletedFoldersToCheck)
            {
                int matchesCount = 0;
                Dictionary<AbsPath, FsObject<AbsPath>> deletedContent = pair.Item2;
                deleted = new List<FsObject<AbsPath>>();
                newObjects = new List<FsObject<AbsPath>>();
                modified = new List<FsFile<AbsPath>>();
                foreach (var descriptor in indexedObjects.Values)
                {

                    FsObject<AbsPath> fsObject;

                    if (deletedContent.TryGetValue(descriptor.Path, out fsObject) )
                    {
                        if(!fsObject.Equals(descriptor))
                        {
                            modified.Add((FsFile<AbsPath>) descriptor);
                        }
                        else
                        {
                            matchesCount++;
                        }
                        
                    }
                    else
                    {
                        newObjects.Add(descriptor);
                    }
                }
                foreach (var descriptor in deletedContent.Values)
                {

                    FsObject<AbsPath> fsObject;

                    if (!indexedObjects.TryGetValue(descriptor.Path, out fsObject))
                    {
                        deleted.Add(descriptor);
                    }

                }

                if (modified.Count + deleted.Count +newObjects.Count< 2 * matchesCount)
                {
                    resultPath = pair.Item1;
                    return true;
                }
                // var folder = table.Table[folderPath];


            }

            return false;
        }
        #endregion

//        private static List<AbsPath> SortedDeletionList(TimedMap<TimedFolderDeletion> deletedMap, AbsPath firstMatchPath)
//        {
//            deletedMap.
//            List<AbsPath> folderList = new List<AbsPath>(deletedMap.Keys);
//            string name = Path.GetFileName(firstMatchPath);
//             dodanie do listy wszystkich potencjalnych folderow);
//             sortowanie aby folder o nazwie takiej samej jak ten folder
//             znalazl sie na poczcatku jako najbardziej prawdopodobny
//            folderList.Sort((one, two) =>
//            {
//                string n = Path.GetFileName(one);
//                if (n != null && n.Equals(name))
//                {
//                    return -1;
//                }
//                return 1;
//            });
//            return folderList;
//        }
        private void FolderCreated(AbsPath fullPath)
        {
  
            var folderList = 
                _deletedFolders.Select(pair => Tuple.Create(pair.Key, pair.Value.IndexedContent)).ToList();
          
            string name = Path.GetFileName(fullPath);
            // dodanie do listy wszystkich potencjalnych folderow);
            // sortowanie aby folder o nazwie takiej samej jak ten folder
            // znalazl sie na poczcatku jako najbardziej prawdopodobny
            folderList.Sort((one, two) =>
            {
                string n = Path.GetFileName(one.Item1);
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


        


        #region FileSystemWatcher events

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

        #endregion

       
        #region Helpers

        private interface IOutTimedMap<out TAction>
        {
            bool Remove(AbsPath par);
            
        }
        private class TimedMap<TAction> : Dictionary<AbsPath, TAction>, IOutTimedMap<TAction>
        {
            
        }

        private static void RemoveTimedEvent(IOutTimedMap<TimedAction> dictionary, TimedAction action)
        {
            action.Stop();
            dictionary.Remove(action.Path);
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
        #endregion

        #region Nested type: TimedAction
        
        public class TimedAction
        {
            private readonly FsObject<AbsPath> _descriptor;
            private readonly Timer _tickTimer;
            private bool _stopped;

            public bool IsStopped
            {
                get { return _stopped; }
            }

            public TimedAction(FsObject<AbsPath> descriptor, TimerCallback callback, long dueTile, long period)
            {
                _descriptor = descriptor;
                _tickTimer = new Timer(callback, this, dueTile, period);
            }

            public FsObject<AbsPath> Descriptor
            {
                get { return _descriptor; }
            }
            public AbsPath Path
            {
                get
                {
                    return _descriptor.Path;
                }
            }
            public void Stop()
            {
                if (_stopped)throw new InvalidOperationException();
                _stopped = true;
                _tickTimer.Dispose();
            }
        }
        #endregion

        public class TimedFolderDeletion : TimedAction
        {
            private Dictionary<AbsPath, FsObject<AbsPath>> _indexedContent;

            public TimedFolderDeletion(FsObject<AbsPath> descriptor, TimerCallback callback, Dictionary<AbsPath, FsObject<AbsPath>> indexedContent)
                : base(descriptor, callback, DeleteWaitTime, Timeout.Infinite)
            {
                _indexedContent = indexedContent;
            }

            public Dictionary<AbsPath, FsObject<AbsPath>> IndexedContent
            {
                get { return _indexedContent; }
            }
        }
    }
}