// Author: Piotr Trzpil

namespace FileModule
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    #region Usings

    #endregion

    public class GroupModel
    {
        private readonly List<string> _folderList;

        public GroupModel(IEnumerable<string> folders)
        {
            _folderList = new List<string>(folders);
        }

        public List<string> FolderList
        {
            get { return _folderList; }
        }
    }
}