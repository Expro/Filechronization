// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    #endregion

    public class IndexingJob : TimedAction
    {
        private readonly AbsPath _absDirPath;
     //   private readonly TimedAction _timing;

//        private readonly LinkedList<AbsPath> _objectsList;

        private readonly CancellationTokenSource _tokenSource;
        private IndexedObjects _indexedObjects;

        public IndexingJob(AbsPath absDirPath, RelPath relDirPath, List<IndexedObjects> folderList, TimerCallback callback, long dueTime)
            : base(new FsFolder<RelPath>(relDirPath),callback, dueTime, Timeout.Infinite)
        {
            _absDirPath = absDirPath;

            _indexedObjects = new IndexedObjects(relDirPath);
            _tokenSource = new CancellationTokenSource();
//            _objectsList = new LinkedList<AbsPath>();

            DeletedContentList = folderList;
        }

        public AbsPath AbsDirPath
        {
            get { return _absDirPath; }
        }

        public List<IndexedObjects> DeletedContentList
        {
            get; private set;
        }

        public IEnumerable<IndexedObjects.DescriptorPair> RelativeDescriptorPairs
        {
            get
            {
                return _indexedObjects.RelativeDescriptorPairs;
            }
      
        }

        public RelPath RootDir
        {
            get { return _indexedObjects.RootDir; }
            
        }

        public IndexedObjects Index
        {
            get {return _indexedObjects; }
            
        }
        public override TimedAction Clone()
        {
            return new IndexingJob(_absDirPath, _indexedObjects.RootDir, DeletedContentList, _callback, _dueTime);
        }

        
//        public TimedAction Timing
//        {
//            get { return _timing; }
//        }

        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        public void IndexAll()
        {
            _indexedObjects.AddAllFromFileSystem(_absDirPath);


//            _tokenSource.Token.ThrowIfCancellationRequested();
//            var list = new LinkedList<AbsPath>();
//            AddAll(_absDirPath, list);
//            GetAllData(list);
   //         Finished(this);
        }
        
//        private void GetAllData(IEnumerable<AbsPath> list)
//        {
//            foreach (AbsPath absPath in list)
//            {
//                _tokenSource.Token.ThrowIfCancellationRequested();
//                Index.Add(absPath.RelativeTo(_absDirPath), FsObject<AbsPath>.ReadFrom(absPath).RelativeTo(_absDirPath));
//            }
//        }
//
//
//
//        private void AddAll(string dir, LinkedList<AbsPath> objects)
//        {
//            string[] files = Directory.GetFiles(dir, "*");
//
//            foreach (string s in files)
//            {
//                AddFile((AbsPath)s, objects);
//            }
//
//            string[] dirs = Directory.GetDirectories(dir, "*");
//            foreach (string s in dirs)
//            {
//                AddFile((AbsPath)s, objects);
//                AddAll(s, objects);
//            }
//        }
//
//        private void AddFile(AbsPath path, LinkedList<AbsPath> objects)
//        {
//            _tokenSource.Token.ThrowIfCancellationRequested();
//
//            objects.AddLast(path);
//        }
    }
}