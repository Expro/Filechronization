// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;
    using System.Timers;
    using Timer = System.Timers.Timer;

    #endregion

    public class IndexingJob : TimedAction
    {
        private readonly AbsPath _absDirPath;
        //   private readonly TimedAction _timing;

//        private readonly LinkedList<AbsPath> _objectsList;

        private readonly BlockingCollection<FileEvent> _eventQueue;
        private readonly Queue<RelPath> _toRemove;
        private readonly IndexedObjects _indexedObjects;

        private readonly Timer _timer;
        private readonly Timer _timer2;

        private QueingThread _queue;
        private readonly CancellationTokenSource _tokenSource;

        private const int FinishDelay = 200;

//        private SyncObject _syncObject;
        private bool _timerFired;
        private bool _timerFired2;
//        private class SyncObject : ISynchronizeInvoke
//        {
//            public bool TimerStopped;
//        }

        public IndexingJob(AbsPath absDirPath, RelPath relDirPath, List<IndexedObjects> folderList, TimerCallback callback, long dueTime, QueingThread queue)
            : base(new FsFolder<RelPath>(relDirPath), callback, dueTime, Timeout.Infinite)
        {
            _absDirPath = absDirPath;
            _queue = queue;
            _indexedObjects = new IndexedObjects(relDirPath);
            _tokenSource = new CancellationTokenSource();

            _eventQueue = new BlockingCollection<FileEvent>();
            DeletedContentList = folderList;

            _timer = new Timer();
            _timer.Elapsed += Finish;
            _timer.AutoReset = false;
            _timer.Interval = FinishDelay;

            _timer2 = new Timer();
            _timer2.Elapsed += Finish2;
            _timer2.AutoReset = false;
            _timer2.Interval = FinishDelay;
        }

        #region Properties

        public AbsPath AbsDirPath
        {
            get { return _absDirPath; }
        }

        public List<IndexedObjects> DeletedContentList { get; private set; }

        public IEnumerable<IndexedObjects.DescriptorPair> RelativeDescriptorPairs
        {
            get { return _indexedObjects.RelativeDescriptorPairs; }
        }

        public RelPath RootDir
        {
            get { return _indexedObjects.RootDir; }
        }

        public IndexedObjects Index
        {
            get { return _indexedObjects; }
        }

        #endregion

        public override TimedAction Clone()
        {
            return new IndexingJob(_absDirPath, _indexedObjects.RootDir, DeletedContentList, _callback, _dueTime, _queue);
        }

        /// <summary>
        ///   Adds to the queue an event that happened inside the folder while indexing.
        ///  Warning: Should be invoked on _queue thread.
        /// </summary>
        /// <param name = "path">Path to an object, relative to indexing root.</param>
        /// <param name = "eventType"></param>
        public bool AddEvent(RelPath path, FileEvents eventType)
        {
            if (!_timerFired)
            {
                _eventQueue.Add(new FileEvent(path, eventType));
                _timer.Interval = FinishDelay;
                return true;
            }
            if (!_timerFired2)
            {
                _toRemove.Enqueue(path);
                return false;
            }
            return false;
        }


        public void StartJob()
        {
            ICollection<AbsPath> notFoundList = _indexedObjects.AddAllFromFileSystem(_absDirPath);

            foreach (AbsPath absPath in notFoundList)
            {
                try
                {
                    _indexedObjects.AddFile(FsFile<AbsPath>.LoadFrom(absPath).RelativeTo(_absDirPath));
                }
                catch (FileNotFoundException e)
                {
                    // do nothing
                    // what else can we do?
                }
            }
            
            _queue.Add(() => _timer.Start());
           

//            _tokenSource.Token.ThrowIfCancellationRequested();
//            var list = new LinkedList<AbsPath>();
//            AddAll(_absDirPath, list);
//            GetAllData(list);
            //         Finished(this);
        }

//        public TimedAction Timing
//        {
//            get { return _timing; }
//        }

        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        [ForeignThreadEntryPoint]
        private void Finish(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _queue.Add(() =>
            {
                if(!_timerFired)
                {
                    _timerFired = true;

     //           TODO: przerzucic na inny watek:

                    foreach (FileEvent fileEvent in _eventQueue)
                    {
                        switch (fileEvent.EventType)
                        {
                            case FileEvents.Created:
                            {
                                try
                                {
                                    _indexedObjects.AddObject(LoadFile(fileEvent.Path));
                                }
                                catch (FileNotFoundException)
                                {
                                    // it was probably already deleted
                                }
                                
                                break;
                            }
                            case FileEvents.Changed:
                            {

                                try
                                {
                                    _indexedObjects.ChangeFile((FsFile<RelPath>) LoadFile(fileEvent.Path));
                                }
                                catch (FileNotFoundException)
                                {
                                    // it was probably already deleted
                                }
                                break;
                            }
                            case FileEvents.Deleted:
                            {
                                _indexedObjects.Remove(fileEvent.Path);
                                break;
                            }
                        }
                    }

                    _timer2.Start();


                }
                
            });
            
        }
        private FsObject<RelPath> LoadFile(RelPath path)
        {
            AbsPath absPath = path.AbsoluteIn(_absDirPath);
            return FsObject<AbsPath>.ReadFrom(absPath).RelativeTo(_absDirPath);
        }
        [ForeignThreadEntryPoint]
        private void Finish2(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _queue.Add(() =>
            {
                _timerFired2 = true;
                foreach (var relPath in _toRemove)
                {
                    _indexedObjects.Remove(relPath);
                }


                

            });


        }

        #region Nested type: FileEvent

        private class FileEvent
        {
            private readonly FileEvents _eventType;
            private readonly RelPath _path;

            public FileEvent(RelPath path, FileEvents eventType)
            {
                _path = path;
                _eventType = eventType;
            }

            #region Properties

            public RelPath Path
            {
                get { return _path; }
            }

            public FileEvents EventType
            {
                get { return _eventType; }
            }

            #endregion
        }

        #endregion

    }
}