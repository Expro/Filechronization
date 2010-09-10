/*
 * 
 * Author: Piotr Trzpil
 * 
 */

namespace Filechronization.UserManagement.Messages
{
	#region
	using Modularity.Messages;
	#endregion
	
    public class UsersInfo: Message
    {
        private Users pUsers;
		
        public UsersInfo(User sender, Users users)
        {
            pUsers = users;
        }
		
        public Users users
        {
            get
            {
                return pUsers;	
            }
        }
    }
}


