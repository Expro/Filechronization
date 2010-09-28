namespace FileModule
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    public class IndexingJob
    {
        private readonly AbsPath _dir;
        private readonly CancellationTokenSource _tokenSource;

        private Dictionary<AbsPath, FsObject<AbsPath>> _table;
        private LinkedList<string> _objectsList;
        public Dictionary<AbsPath, FsObject<AbsPath>> Table
        {
            get
            {
//                if (_tokenSource.Token.IsCancellationRequested || !finished)
                
                return _table;
            }
        }

        public AbsPath Dir
        {
            get { return _dir; }
        }

        public object UserObject
        {
            get; set;
        }

        public event Action<IndexingJob> Finished;

        public IndexingJob(AbsPath dir)
        {
            _dir = dir;
            _tokenSource = new CancellationTokenSource();

            _table = new Dictionary<AbsPath, FsObject<AbsPath>>();
            _objectsList = new LinkedList<string>();
        }



        public void Cancel()
        {
            _tokenSource.Cancel();
        }
        public void IndexAll()
        {
            _tokenSource.Token.ThrowIfCancellationRequested();
            AddAll(_dir);
            GetAllData();
            Finished(this);
        }
        private void GetAllData()
        {
            foreach (var path in _objectsList)
            {
                AbsPath absPath = (AbsPath) path;
                _tokenSource.Token.ThrowIfCancellationRequested();
                _table.Add(absPath, FsObject<AbsPath>.NewLocal(absPath));
            }
            
        }
        private void AddAll(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*");

            foreach (string s in files)
            {
                AddFile(s);
            }

            string[] dirs = Directory.GetDirectories(dir, "*");
            foreach (string s in dirs)
            {
                AddFile(s);
                AddAll(s);
            }
        }

        private void AddFile(string path)
        {
            _tokenSource.Token.ThrowIfCancellationRequested();

            _objectsList.AddLast(path);
            
            
        }
    }
}