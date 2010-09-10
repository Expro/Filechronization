namespace FileModule
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    public class GroupModel
    {
        public List<string> folderList;

        public GroupModel(IEnumerable<string> folders)
        {
            folderList = new List<string>();
            folderList.AddRange(folders);
        }
    }
}