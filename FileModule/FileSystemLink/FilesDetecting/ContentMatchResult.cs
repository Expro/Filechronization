namespace FileModule
{

    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    public class ContentMatchResult
    {
        private RelPath _matchingDeletedFolderPath;
        private List<FsObject<RelPath>> _created;
        private List<FsObject<RelPath>> _deleted;
        private List<FsFile<RelPath>> _modified;
        private List<FsObject<RelPath>> _moved;

        public ContentMatchResult(RelPath matchingDeletedFolderPath)
        {
            _created = new List<FsObject<RelPath>>();
            _deleted = new List<FsObject<RelPath>>();
            _modified = new List<FsFile<RelPath>>();
            _moved= new List<FsObject<RelPath>>();
            _matchingDeletedFolderPath = matchingDeletedFolderPath;
        }

        public RelPath MatchingDeletedFolderPath
        {
            get { return _matchingDeletedFolderPath; }
        
        }

        public List<FsObject<RelPath>> Created
        {
            get { return _created; }
        }

        public List<FsObject<RelPath>> Deleted
        {
            get { return _deleted; }
        }

        public List<FsFile<RelPath>> Modified
        {
            get { return _modified; }
        }

        public List<FsObject<RelPath>> Moved
        {
            get
            {
                return _moved;
            }
            
        }
    }
}