// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    #endregion

    public class IndexingJob : IndexedObjects
    {
        private readonly AbsPath _absDirPath;

        private readonly LinkedList<AbsPath> _objectsList;

        private readonly CancellationTokenSource _tokenSource;

        public IndexingJob(AbsPath absDirPath, RelPath relDirPath, List<IndexedObjects> folderList)
            : base(relDirPath)
        {
            _absDirPath = absDirPath;

            _tokenSource = new CancellationTokenSource();
            _objectsList = new LinkedList<AbsPath>();

            UserObject = folderList;
        }

        public AbsPath AbsDirPath
        {
            get { return _absDirPath; }
        }

        public List<IndexedObjects> UserObject
        {
            get; private set;
        }

   

        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        public void IndexAll()
        {
            _tokenSource.Token.ThrowIfCancellationRequested();
            AddAll(_absDirPath);
            GetAllData();
   //         Finished(this);
        }

        private void GetAllData()
        {
            foreach (AbsPath absPath in _objectsList)
            {
                _tokenSource.Token.ThrowIfCancellationRequested();
                Index.Add(absPath.RelativeTo(_absDirPath), FsObject<AbsPath>.ReadFrom(absPath).RelativeTo(_absDirPath));
            }
        }

        private void AddAll(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*");

            foreach (string s in files)
            {
                AddFile((AbsPath) s);
            }

            string[] dirs = Directory.GetDirectories(dir, "*");
            foreach (string s in dirs)
            {
                AddFile((AbsPath) s);
                AddAll(s);
            }
        }

        private void AddFile(AbsPath path)
        {
            _tokenSource.Token.ThrowIfCancellationRequested();

            _objectsList.AddLast(path);
        }
    }
}