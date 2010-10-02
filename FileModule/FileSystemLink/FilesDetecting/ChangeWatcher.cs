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
    /// <summary>
    /// Handles lower level of detecting filesystem changes:
    /// Subscribes to FileSystemWatcher and translates its events 
    /// to higher level, especially: files and folders moves
    /// </summary>
    public class ChangeWatcher : NetworkContextModule
    {


        private const long DeleteWaitTime = 500;
        private const long NewFileCheckPeriod = 1000;
        private const long PairedCheckWaitTime = 100;

        private readonly Dictionary<RelPath, IndexingJob> _activeIndexings;

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

        public ChangeWatcher(NetworkContext netContext)
            : base(netContext)
        {
            _deletedFolders = new TimedMap<TimedFolderDeletion>();
            _deletedFiles = new TimedMap<TimedAction>();

            _activeIndexings = new Dictionary<RelPath, IndexingJob>();
            _trackedNewFiles = new TimedMap<TimedAction>();
            _pairChangeTimings = new TimedMap<TimedAction>();

            _watcher = new FileSystemWatcher(MainPath.ToString());

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

        public event Action<FsObject<RelPath>, FsObject<RelPath>> MovedRenamed;
        public event Action<FsObject<RelPath>, FsObject<RelPath>> Replaced;

        public event Action<FsObject<RelPath>> Deleted;
        public event Action<FsObject<RelPath>> Created;
        public event Action<FsFile<RelPath>, FsFile<RelPath>> Modified;


        #region FileEvents

        private void FileChanged(AbsPath fullPath)
        {
            Console.Out.WriteLine(">>>> CH: " + fullPath);
            RelPath relPath = fullPath.RelativeTo(MainPath);
            TimedAction timed;
            if (_trackedNewFiles.TryGetValue(relPath, out timed))
            {
                // File is new, not yet in the main index
                TrackedFileCheck(timed);
            }
            else 
            {
                FsFile<RelPath> oldFileProps;
                try
                {
                    oldFileProps = (FsFile<RelPath>)FileIndex.GetObject(relPath);
                }
                catch (Exception)
                {
                    // Ancestor folder is being indexed.
                    return;
                }
                
                FsFile<RelPath> newFileProps = FsFile<AbsPath>.LoadFrom(fullPath).RelativeTo(MainPath);

                if (_pairChangeTimings.TryGetValue(relPath, out timed))
                {
                    RemoveTimedEvent(_pairChangeTimings, timed);
                    Modified(oldFileProps, newFileProps);
                }
                else
                {
                    timed = new TimedAction(newFileProps, FireChanged, PairedCheckWaitTime, Timeout.Infinite);
                    _pairChangeTimings.Add(relPath, timed);
                    return;
                }

                
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
                    FsFile<RelPath> oldFileProps = (FsFile<RelPath>)FileIndex.GetObject(timing.Path);
                    Modified(oldFileProps, (FsFile<RelPath>)timing.Descriptor);
                }
                
            });
            

        }

       

        private void FileCreated(AbsPath fullPath)
        {
            FsFile<RelPath> fsFile = FsFile<AbsPath>.LoadFrom(fullPath).RelativeTo(MainPath);

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

        private void TrackNewFile(FsFile<RelPath> fsFile)
        {
            
            if(!IsFileBeingUsed(fsFile.Path))
            {
                // Empty file or moved from outside.
                Created(fsFile);
            }
            else
            {
                Console.Out.WriteLine("Used");
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
            FsObject<RelPath> fsObject = FileIndex.GetObject(path.RelativeTo(MainPath));


            if (fsObject is FsFile<RelPath>)
            {
                TimedAction events = new TimedAction(fsObject, DeleteTimerEnded, DeleteWaitTime, Timeout.Infinite);
                _deletedFiles.Add(fsObject.Path, events);
            }
            else
            {
                var indexed = FileIndex.GetIndexedFor(fsObject.Path);
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

        

        private void IndexingJobCallback(IndexingJob newIndexed, Exception exception)
        {
            _queue.Add(() =>
            {
                IndexingJob indexingJob;
                if (_activeIndexings.TryGetValue(newIndexed.RootDir, out indexingJob) && indexingJob == newIndexed)
                {
                    _activeIndexings.Remove(newIndexed.RootDir);
                }

                if (exception==null)
                {
         
                    TimedAction delEvent = null;
      
                    ContentMatchResult matchResult;
                 //   IndexedObjects newIndexed = finishedIndexing;

                    if (TryFindMatch(newIndexed, newIndexed.UserObject , out matchResult))
                    {
                        delEvent = _deletedFolders[matchResult.MatchingDeletedFolderPath];
                    }

                    if (delEvent != null && !delEvent.IsStopped)
                    {

                        RemoveTimedEvent(_deletedFolders, delEvent);

                        MovedRenamed(new FsFolder<RelPath>(matchResult.MatchingDeletedFolderPath), new FsFolder<RelPath>(newIndexed.RootDir));
                        foreach (FsObject<RelPath> fsObject in matchResult.Deleted)
                        {
                            Deleted(fsObject);
                        }
                        foreach (FsFile<RelPath> fsObject in matchResult.Modified)
                        {
                            Modified((FsFile<RelPath>)FileIndex.GetObject(fsObject.Path), fsObject);

                        }
                        foreach (FsObject<RelPath> newObject in matchResult.Created)
                        {
                            Created(newObject);
                        }
                    }
                    else
                    {
                        Created(new FsFolder<RelPath>(newIndexed.RootDir));
                        // Stworzenie nowego
                        foreach (var obj in newIndexed.ValuesRelativeToMainPath)
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
        /// <param name="deletedFolders">List of pairs of folder path and indexed folder content as Dictionary</param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryFindMatch(IndexedObjects indexedObjects, List<IndexedObjects> deletedFolders,
                                  out ContentMatchResult result)
        {
            result = default(ContentMatchResult);

            if (deletedFolders.Count == 0)
            {
                return false;
            }
//            List<FsObject<AbsPath>> deleted = new List<FsObject<AbsPath>>();
//            List<FsObject<AbsPath>> newAndModified = new List<FsObject<AbsPath>>();

            foreach (var deletedContent in deletedFolders)
            {
                int matchesCount = 0;
                result = new ContentMatchResult();
                foreach (var descriptor in indexedObjects.Index.Values)
                {

                    FsObject<RelPath> fsObject;

                    if (deletedContent.Index.TryGetValue(descriptor.Path, out fsObject))
                    {
                        if(!fsObject.Equals(descriptor))
                        {
                            result.Modified.Add((FsFile<RelPath>) descriptor.RelativeIn(indexedObjects.RootDir));
                        }
                        else
                        {
                            matchesCount++;
                        }
                        
                    }
                    else
                    {
                        result.Created.Add(descriptor.RelativeIn(indexedObjects.RootDir));
                    }
                }
                foreach (var descriptor in deletedContent.Index.Values)
                {

                    FsObject<RelPath> fsObject;

                    if (!indexedObjects.Index.TryGetValue(descriptor.Path, out fsObject))
                    {
                        result.Deleted.Add(descriptor.RelativeIn(indexedObjects.RootDir));
                    }

                }

                if (result.Modified.Count + result.Deleted.Count + result.Created.Count < 2 * matchesCount)
                {
                    result.MatchingDeletedFolderPath = deletedContent.RootDir;
                    return true;
                }
                // var folder = table.Table[folderPath];


            }

            return false;
        }
        #endregion

        private void FolderCreated(AbsPath fullPath)
        {
            RelPath relPath = fullPath.RelativeTo(MainPath);
            var folderList = _deletedFolders.Values.Select(deletion => deletion.IndexedContent).ToList();
//                _deletedFolders.Select(pair => Tuple.Create(pair.Key, pair.Value.IndexedContent)).ToList();
          
            string name = Path.GetFileName(fullPath);
            // dodanie do listy wszystkich potencjalnych folderow);
            // sortowanie aby folder o nazwie takiej samej jak ten folder
            // znalazl sie na poczcatku jako najbardziej prawdopodobny
            folderList.Sort((one, two) =>
            {
                string n = Path.GetFileName(one.RootDir.ToString());
                if (n != null && n.Equals(name))
                {
                    return -1;
                }
                return 1;
            });



            IndexingJob indexing = new IndexingJob(fullPath,relPath, folderList);
            _activeIndexings.Add(relPath, indexing);
            FileIndex.RunIndexingJob(indexing, IndexingJobCallback);
        }

        private bool TryResetIndexings(RelPath relPath)
        {
            IndexingJob indexing;
            ICollection<RelPath> relativeParentFolders = PathUtils.GetRelativeParentFolders(relPath);
            foreach (RelPath ancestorPath in relativeParentFolders)
            {
                if (_activeIndexings.TryGetValue(ancestorPath, out indexing))
                {
                    indexing.Cancel();
                    IndexingJob indexing2 = new IndexingJob(indexing.AbsDirPath, relPath, indexing.UserObject);
                    _activeIndexings.Add(relPath, indexing2);
                    FileIndex.RunIndexingJob(indexing2, IndexingJobCallback);
                    return true;
                }
            }
            
            return false;
        }


        


        #region FileSystemWatcher events

        private void OnRenamed(object source, RenamedEventArgs eArgs)
        {
            try
            {
                FsObject<RelPath> oldDescr = FileIndex.GetObject(((AbsPath) eArgs.OldFullPath).RelativeTo(MainPath));
                FsObject<RelPath> newDescr = FsObject<AbsPath>.ReadFrom((AbsPath)eArgs.FullPath).RelativeTo(MainPath);

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
                            Console.Out.WriteLine(">>>> CR: " + eArgs.FullPath);
                            if (TryResetIndexings(fullPath.RelativeTo(MainPath)))
                            {
//                                 nie bawimy sie w takie komplikacje
//
//                                 indeksowanie parenta powinno zlapac stworzenie tego
                                return;
                            }
                            if (File.Exists(fullPath))
                            {
                                FileCreated(fullPath);
                              
                            }
                            else if (Directory.Exists(fullPath))
                            {
                                FolderCreated(fullPath);
           
                            }
                        });


                        break;
                    }


                    case WatcherChangeTypes.Deleted:
                    {
                        Console.Out.WriteLine(">>>> DE: " + eArgs.FullPath);
                        _queue.Add(() => ObjectDeleted((AbsPath) eArgs.FullPath));

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
     
                LoggingService.Trace.Error(e.ToString(), sender: this);
            }

        }

        #endregion

       
        #region Helpers

        private interface IOutTimedMap<out TAction>
        {
            bool Remove(RelPath par);
            
        }
        private class TimedMap<TAction> : Dictionary<RelPath, TAction>, IOutTimedMap<TAction>
        {
            
        }

        private static void RemoveTimedEvent(IOutTimedMap<TimedAction> dictionary, TimedAction action)
        {
            action.Stop();
            dictionary.Remove(action.Path);
        }
        private bool IsFileBeingUsed(RelPath path)
        {
            try
            {
                // Check if is used by another process
                new FileInfo(path.AbsoluteIn(MainPath)).OpenWrite().Close();
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
        #endregion

        #region Nested type: TimedAction

        private class TimedAction
        {
            private readonly FsObject<RelPath> _descriptor;
            private readonly Timer _tickTimer;
            private bool _stopped;

            public bool IsStopped
            {
                get { return _stopped; }
            }

            public TimedAction(FsObject<RelPath> descriptor, TimerCallback callback, long dueTile, long period)
            {
                _descriptor = descriptor;
                _tickTimer = new Timer(callback, this, dueTile, period);
            }

            public FsObject<RelPath> Descriptor
            {
                get { return _descriptor; }
            }
            public RelPath Path
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

        private class TimedFolderDeletion : TimedAction
        {
            private IndexedObjects _indexedContent;

            public TimedFolderDeletion(FsObject<RelPath> descriptor, TimerCallback callback, IndexedObjects indexedContent)
                : base(descriptor, callback, DeleteWaitTime, Timeout.Infinite)
            {
                _indexedContent = indexedContent;
            }

            public IndexedObjects IndexedContent
            {
                get { return _indexedContent; }
            }
        }
    }
}