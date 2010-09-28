namespace FileModule
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    public class GroupModel
    {
        private List<string> _folderList;

        public List<string> FolderList
        {
            get { return _folderList; }
        }

        public GroupModel(IEnumerable<string> folders)
        {
            _folderList = new List<string>(folders);
           
        }
    }
}