using Filechronization.UserManagement;

namespace FileModule
{
	#region Usings
	
	using System.Collections.Generic;

	#endregion

	public class NetworkModel
	{
		public List<GroupModel> GroupList;
		public MainStoragePath MainPath;
		public Users UserList;

		public NetworkModel(string path)
		{
			MainPath = new MainStoragePath((AbsPath) path);
			GroupList = new List<GroupModel>();
		}
	}
}