namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    #endregion

    public class FileWatcher
    {
        #region Delegates

        public delegate void FilePathChangedEventHandler(string sourcePath, string targetPath);

        public delegate void FileCreatedChangedEventHandler(FileSystemObjectDescriptor path);
        public delegate void FileDeletedEventHandler(string relativePath);

        #endregion

        private readonly Dictionary<string, FileEvents> dict;
        private readonly NewFileModule fileModule;
        //private readonly string folderPath;
        //private FileTable fileTable;

        private readonly QueingThread queue;
        private readonly FileSystemWatcher watcher;

        public FileWatcher(NewFileModule fileModule)
        {
            this.fileModule = fileModule;
            dict = new Dictionary<string, FileEvents>();
            // folderPath = path;
            watcher = new FileSystemWatcher(fileModule.Network.MainPath.Get);

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

        public event FilePathChangedEventHandler MovedRenamed;
        public event FilePathChangedEventHandler Replaced;

        public event FileDeletedEventHandler Deleted;
        public event FileCreatedChangedEventHandler Created;
        public event FileCreatedChangedEventHandler Modified;


        private void FileChanged(string path)
        {
            queue.Add(delegate { AddEvent(path, WatcherChangeTypes.Changed); });
        }

        private void FileDeleted(string path)
        {
            queue.Add(delegate { AddEvent(path, WatcherChangeTypes.Deleted); });
        }

        private FileEvents AddEvent(string path, WatcherChangeTypes eventType)
        {
            FileEvents events;
            if (dict.TryGetValue(path, out events))
            {
            }
            else
            {
                FileSystemObjectDescriptor descr = null;
                if (eventType != WatcherChangeTypes.Deleted)
                {
                    descr = FileSystemObjectDescriptor.New(fileModule.Network.MainPath, path);
                    //new FileDescriptor(fileModule.Network.MainPath, path);
                }
                events = new FileEvents(descr, path, FileEventsTimerEnded);
                dict.Add(path, events);
            }

            events.queue.Enqueue(eventType);
            return events;
        }

        private void RemoveFileEvents(string path)
        {
            FileEvents events;
            if (dict.TryGetValue(path, out events))
            {
                events.timer.Dispose();
                dict.Remove(path);
            }
        }

        private void FileEventsTimerEnded(object state)
        {
            queue.Add(delegate
            {
                var events = (FileEvents) state;
                dict.Remove(events.filePath);

                while (events.queue.Count != 0)
                {
                    var eventType = events.queue.Dequeue();

                    if (eventType == WatcherChangeTypes.Deleted)
                    {
                        Deleted(events.filePath);
                    }
                    else if (eventType == WatcherChangeTypes.Created)
                    {

                        Created(events.descriptor);
                    }
                    else if (eventType == WatcherChangeTypes.Changed)
                    {
                        while (events.queue.Count != 0 &&
                               events.queue.Peek() == WatcherChangeTypes.Changed)
                        {
                            events.queue.Dequeue();
                        }
                        Modified(events.descriptor);
                    }
                }

                if (events.queue.Contains(WatcherChangeTypes.Deleted))
                {
                    if (Deleted != null)
                    {
                        Deleted(events.filePath);
                    }
                }
                if (events.queue.Contains(WatcherChangeTypes.Created))
                {
                    if (Created != null)
                    {
                        Created(events.descriptor);
                    }
                }
            });
        }

        private void FileCreated(string path)
        {
            queue.Add(delegate
            {
                FileEvents newFile = AddEvent(path, WatcherChangeTypes.Created);

                // Wyszukiwanie zdarzen usuniecia w celu indentyfikacji zmiany nazwy lub przeniesienia
                foreach (var other in dict.Values)
                {
                    if (newFile != other && other.queue.Contains(WatcherChangeTypes.Deleted)
                        && fileModule.tableOverseer.GetFile(other.filePath).Equals(newFile.descriptor))
                    {
                        if (newFile.queue.Contains(WatcherChangeTypes.Deleted))
                        {
                            Replaced(other.filePath, newFile.filePath);
                        }
                        else
                        {
                            MovedRenamed(other.filePath, newFile.filePath);
                        }

                        RemoveFileEvents(other.filePath);
                        RemoveFileEvents(newFile.filePath);


                        return;
                    }
                }


                // Jesli jest to folder - trzeba zaindeksowac cala zawartosc
                // Niech wlaczy sie timer na x czasu. Jesli timer dojdzie do konca - rozpocznie sie indeksowanie
                // kazde zdarzenie Create wewnatrz tego folderu niech resetuje timer.

            });
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                {
                    if (Directory.Exists(e.FullPath))
                    {
                        // nic nie rob
                    }
                    else
                    {
                        FileChanged(e.FullPath);
                    }

                    break;
                }
                    
                case WatcherChangeTypes.Created:
                {
                    FileCreated(e.FullPath);
                    break;
                }
                    

                    
                case WatcherChangeTypes.Deleted:
                {
                    FileDeleted(e.FullPath);

                    break;
                }
                    
                default:
                {
                    Console.WriteLine("Default!");
                    break;
                }
                    
            }


            //  Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }


        private void OnRenamed(object source, RenamedEventArgs e)
        {
//            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            queue.Add(delegate { MovedRenamed(e.OldFullPath, e.FullPath); });
        }

        #region Nested type: FileEvents

        public class FileEvents
        {
            public FileSystemObjectDescriptor descriptor;
            public string filePath;
            public Queue<WatcherChangeTypes> queue;
            public Timer timer;


            public FileEvents(FileSystemObjectDescriptor descr, string path, TimerCallback callback)
            {
                filePath = path;
                queue = new Queue<WatcherChangeTypes>();
                descriptor = descr;
                timer = new Timer(callback, this, 500, Timeout.Infinite);
            }
        }

        #endregion
    }
}