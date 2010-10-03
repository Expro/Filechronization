// Author: Piotr Trzpil
#define LOWER_NOTICE
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

    

    /// <summary>
    /// Handles lower level of detecting filesystem changes:
    /// Subscribes to FileSystemWatcher and translates its events 
    /// to higher level, especially: files and folders moves
    /// </summary>
    public class ChangeWatcher : NetworkContextModule
    {
        public const long DeleteWaitTime = 500;

        private const long FileCreationDelay = 100;
        private const long NewFileCheckPeriod = 1000;
        private const long CheckEventFiringDelay = 100;
        private const long IndexingDelay = 200;

        private readonly TimedMap<IndexingJob> _activeIndexings;

        private readonly TimedMap<TimedFolderDeletion> _deletedFolders;
        private readonly TimedMap<TimedAction> _deletedFiles;

        private readonly TimedMap<TimedNewFileTracking> _trackedNewFiles;

        private readonly TimedMap<TimedAction> _changeDelays;

        private readonly TimedMap<TimedAction> _scheguledIndexings;
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
            _scheguledIndexings = new TimedMap<TimedAction>();
            _activeIndexings = new TimedMap<IndexingJob>();
            _trackedNewFiles = new TimedMap<TimedNewFileTracking>();
            _changeDelays = new TimedMap<TimedAction>();

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
            
            
            RelPath relPath = fullPath.RelativeTo(MainPath);
            TimedNewFileTracking timed;
            if (_trackedNewFiles.TryGetValue(relPath, out timed) && timed.WasCheckedByTimer)
            {
                // File is new, not yet in the main index and was at least once checked by timer.
                TrackedFileCheck(timed);
            }
            else if (!IsAncestorIndexed(relPath))
            {
                
                FsFile<RelPath> newFileProps = FsFile<AbsPath>.LoadFrom(fullPath).RelativeTo(MainPath);
                TimedAction pairedChange;
                if (_changeDelays.TryGetValue(relPath, out pairedChange))
                {
                    RemoveTimedEvent(_changeDelays, pairedChange);
                    _changeDelays.Add(relPath, pairedChange.Clone());
                    //Modified(oldFileProps, newFileProps);
                }
                else
                {
                    pairedChange = new TimedAction(newFileProps, FireChanged, CheckEventFiringDelay, Timeout.Infinite);
                    _changeDelays.Add(relPath, pairedChange);
                    
                }

            }
            
        }
        private bool IsAncestorIndexed(RelPath relPath)
        {
            try
            {
                FileIndex.GetObject(relPath);
                return false;
            }
            catch (Exception)
            {
                // Ancestor folder is being indexed.
                return true;
            }
        }
        private void FireChanged(object state)
        {
            _queue.Add(() =>
            {
                var timing = (TimedAction)state;
                if (!timing.IsTimerStopped)
                {
                    RemoveTimedEvent(_changeDelays, timing);

                    if (!IsAncestorIndexed(timing.Path))
                    {
                        FsFile<RelPath> oldFileProps = (FsFile<RelPath>)FileIndex.GetObject(timing.Path);
                        Modified(oldFileProps, (FsFile<RelPath>)timing.Descriptor);
                    }
                    
                }
                
            });
            

        }

       
        /// <summary>
        /// 1) Check if it was a file move, if not:
        /// 2) Schedule timer for FileCreationDelay
        ///    During that time:
        ///     - FileChanged is ignored.
        ///     - FileDeletion is not fired up, but ends tracking of this file.
        /// 3) When timer fires, check if is being used:
        ///     if not: fire Created and end tracking.
        ///     else: schedule for periodical check.
        /// </summary>
        /// <param name="fullPath"></param>
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
            catch (InvalidOperationException) // Matching deletion not found - file is new.
            {
                var tracking = new TimedNewFileTracking(fsFile, TrackedFileCheck, FileCreationDelay, NewFileCheckPeriod);
                _trackedNewFiles.Add(fsFile.Path, tracking);
                //       Created(fsFile);
                
            }
        }

        
        private void TrackedFileCheck(object state)
        {
            _queue.Add(() =>
            {
                var tracking = (TimedNewFileTracking)state;
                tracking.WasCheckedByTimer = true;

                if (!tracking.IsTimerStopped && !IsFileBeingUsed(tracking.Descriptor.Path))
                {
                    RemoveTimedEvent(_trackedNewFiles, tracking);
                    Created(tracking.Descriptor);
                }
            });
            

        }

        #endregion




        #region Common: Deleted
        private void ObjectDeleted(AbsPath path)
        {
            RelPath relPath = path.RelativeTo(MainPath);

            if(!TryResetIndexings(relPath))
            {
                if(CancelDelayedEvents(relPath))
                {
                    // File is new and not yet in the table
                    return;
                }


                FsObject<RelPath> fsObject = FileIndex.GetObject(relPath);
//                try
//                {
//                    fsObject
//                }
//                catch (KeyNotFoundException)
//                {
                    // File is new and not yet in the table
//                    return;
//                }
                


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
            }
            
        }
        /// <summary>
        /// Cancels 'new file delay' and possibly 'change delay'
        /// </summary>
        /// <param name="relPath"></param>
        /// <returns>True if file was in 'new file delay' state </returns>
        private bool CancelDelayedEvents(RelPath relPath)
        {
            TimedNewFileTracking timedNewFileTracking;
            if (_trackedNewFiles.TryGetValue(relPath, out timedNewFileTracking))
            {
                RemoveTimedEvent(_trackedNewFiles, timedNewFileTracking);
                return true; // If file is in this stage, cant be in _changeDelays anyway.
            }

            TimedAction timedAction;
            if (_changeDelays.TryGetValue(relPath, out timedAction))
            {
                RemoveTimedEvent(_changeDelays, timedAction);
            }
            return false;
        }

        private void DeleteTimerEnded(object state)
        {
            _queue.Add(() =>
            {
                TimedAction deletion = (TimedAction)state;

                if (!deletion.IsTimerStopped)
                {
                    if (deletion.Descriptor is FsFile<RelPath>)
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
                
                    if (TryFindMatch(newIndexed, newIndexed.DeletedContentList , out matchResult))
                    {
                        delEvent = _deletedFolders[matchResult.MatchingDeletedFolderPath];
                    }

                    if (delEvent != null && !delEvent.IsTimerStopped)
                    {

                        RemoveTimedEvent(_deletedFolders, delEvent);

                        
                        foreach (FsObject<RelPath> fsObject in matchResult.Moved)
                        {
                            RelPath path = fsObject.Path.RelativeIn(matchResult.MatchingDeletedFolderPath);
                            MovedRenamed(FileIndex.GetObject(path), fsObject.RelativeIn(newIndexed.RootDir));
                        }
                        foreach (FsObject<RelPath> fsObject in matchResult.Deleted)
                        {
                            Deleted(fsObject);
                        }
                        foreach (FsFile<RelPath> fsObject in matchResult.Modified)
                        {
                            RelPath path = fsObject.Path.RelativeIn(matchResult.MatchingDeletedFolderPath);
                            Modified((FsFile<RelPath>)FileIndex.GetObject(path), (FsFile<RelPath>) fsObject.RelativeIn(newIndexed.RootDir));

                        }
                        foreach (FsObject<RelPath> newObject in matchResult.Created)
                        {
                            Created(newObject);
                        }
                        MovedRenamed(new FsFolder<RelPath>(matchResult.MatchingDeletedFolderPath), new FsFolder<RelPath>(newIndexed.RootDir));
                    }
                    else
                    {
                        Created(new FsFolder<RelPath>(newIndexed.RootDir));
                        // Stworzenie nowego
                        foreach (var obj in newIndexed.RelativeDescriptorPairs)
                        {
                            Created(obj.RelToMainPath);
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
        private static bool TryFindMatch(IndexingJob indexedObjects, List<IndexedObjects> deletedFolders,
                                  out ContentMatchResult result)
        {
            result = default(ContentMatchResult);

            if (deletedFolders.Count == 0)
            {
                return false;
            }

            foreach (IndexedObjects deletedContent in deletedFolders)
            {
            
                result = new ContentMatchResult(deletedContent.RootDir);
                foreach (IndexedObjects.DescriptorPair descriptorPair in indexedObjects.RelativeDescriptorPairs)
                {
                   
                    FsObject<RelPath> fsObject;

                    if (deletedContent.Index.TryGetValue(descriptorPair.RelToRootDir.Path, out fsObject))
                    {
                        if(fsObject is FsFile<RelPath> && !fsObject.Equals(descriptorPair.RelToRootDir))
                        {
                            result.Modified.Add((FsFile<RelPath>)descriptorPair.RelToRootDir);
                        }
                        else
                        {
                            result.Moved.Add(descriptorPair.RelToRootDir);
                        }
                        
                    }
                    else
                    {
                        result.Created.Add(descriptorPair.RelToMainPath);
                    }
                }
                foreach (IndexedObjects.DescriptorPair descriptor in deletedContent.RelativeDescriptorPairs)
                {

                    FsObject<RelPath> fsObject;

                    if (!indexedObjects.Index.TryGetValue(descriptor.RelToRootDir.Path, out fsObject))
                    {
                        result.Deleted.Add(descriptor.RelToMainPath);
                    }

                }

                if (result.Modified.Count + result.Deleted.Count + result.Created.Count < result.Moved.Count)
                {
                    return true;
                }
                
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


         //   TimedAction timing = new TimedAction(new FsFolder<RelPath>(relPath),StartIndexingCallback, IndexingDelay, Timeout.Infinite);
            IndexingJob indexing = new IndexingJob(fullPath, relPath, folderList, StartIndexingCallback, IndexingDelay);
            _activeIndexings.Add(relPath, indexing);

            

           // _activeIndexings

            
        }

        private void StartIndexingCallback(object state)
        {
            
            _queue.Add(() =>
            {
                
                TimedAction action = (TimedAction)state;
                if(!action.IsTimerStopped)
                {
                    IndexingJob activeIndexing = _activeIndexings[action.Path];
                    action.Stop();
                    FileIndex.RunIndexingJob(activeIndexing, IndexingJobCallback);
                }
                
                
                
                

            });
            
        }

        private bool TryResetIndexings(RelPath relPath)
        {
            IndexingJob indexing;
            ICollection<RelPath> relativeParentFolders = PathUtils.GetRelativeParentFolders(relPath);
            foreach (RelPath ancestorPath in relativeParentFolders)
            {
                if (_activeIndexings.TryGetValue(ancestorPath, out indexing))
                {
                    
//                    {
//                        RemoveTimedEvent(_activeIndexings, indexing);
                        //indexing.Timing.Restart();
//                        _activeIndexings.Add(indexing.RootDir,(IndexingJob) indexing.Clone());
//                    }
                    if (indexing.IsTimerStopped) // Indexing was already strarted
                    {
                        indexing.Cancel();
                   //     TimedAction timing = new TimedAction(new FsFolder<RelPath>(relPath), StartIndexingCallback, IndexingDelay, Timeout.Infinite);
                       // IndexingJob indexing2 = new IndexingJob(indexing.AbsDirPath, relPath, indexing.DeletedContentList, StartIndexingCallback, IndexingDelay);
                       // _activeIndexings.Add(relPath, indexing2);
                      //  FileIndex.RunIndexingJob(indexing2, IndexingJobCallback);
                    }
                    RemoveTimedEvent(_activeIndexings, indexing);
                    //indexing.Timing.Restart();
                    _activeIndexings.Add(indexing.RootDir, (IndexingJob)indexing.Clone());
                    return true;
                }
            }
            
            return false;
        }


        


        #region FileSystemWatcher events

        private void OnRenamed(object source, RenamedEventArgs eArgs)
        {
            _queue.Add(() =>
            {
#if LOWER_NOTICE
Console.Out.WriteLine(">>>> RN: " + eArgs.FullPath);
#endif


                FsObject<RelPath> oldDescr = FileIndex.GetObject(((AbsPath)eArgs.OldFullPath).RelativeTo(MainPath));
                FsObject<RelPath> newDescr = FsObject<AbsPath>.ReadFrom((AbsPath)eArgs.FullPath).RelativeTo(MainPath);

                if (!TryResetIndexings(newDescr.Path))
                {
                    MovedRenamed(oldDescr, newDescr);
                }
            });
                
            
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
                            _queue.Add(() =>
                            {
#if LOWER_NOTICE
Console.Out.WriteLine(">>>> CH: " + eArgs.FullPath);
#endif

                                FileChanged((AbsPath) eArgs.FullPath);
                            });

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
#if LOWER_NOTICE
Console.Out.WriteLine(">>>> CR: " + eArgs.FullPath);
#endif

                            if (TryResetIndexings(fullPath.RelativeTo(MainPath)))
                            {
//                                 nie bawimy sie w takie komplikacje
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
                        
                        _queue.Add(() =>
                        {
#if LOWER_NOTICE
Console.Out.WriteLine(">>>> DE: " + eArgs.FullPath);
#endif

                            ObjectDeleted((AbsPath) eArgs.FullPath);
                        });

                        break;
                    }

                    default:
                    {
                        throw new Exception("Unknown change value: "+eArgs.ChangeType);
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
        public interface ITimedAction
        {
            bool IsStopped
            { get;
            }
            RelPath Path
            { get;
            }

            void Stop();
            void Clone();
        }

        #endregion
    }
}