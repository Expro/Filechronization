namespace FileModule
{

    #region Usings

    using System.Collections.Generic;
    using System.Linq;

    #endregion

    public class ContentMatchResult
    {
        private RelPath _matchingDeletedFolderPath;
        private List<FsObject<RelPath>> _created;
        private List<FsObject<RelPath>> _deleted;
        private List<FsFile<RelPath>> _modified;

        public ContentMatchResult()
        {
            _created = new List<FsObject<RelPath>>();
            _deleted = new List<FsObject<RelPath>>();
            _modified = new List<FsFile<RelPath>>();
        }

        public RelPath MatchingDeletedFolderPath
        {
            get { return _matchingDeletedFolderPath; }
            set { _matchingDeletedFolderPath = value; }
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
    }
}