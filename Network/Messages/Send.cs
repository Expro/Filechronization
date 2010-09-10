/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.Network.Messages
{
	#region
	using global::System;
	using Modularity.Messages;
	using UserManagement;
	#endregion
	
    [Serializable]
    public class Send: Message
    {
        private readonly User _reciver;
        private Message pMessage;
		
        public Send(User reciver, Message message)
        {
            _reciver = reciver;
            pMessage = message;
        }

        public User reciver
        {
            get
            {
                return _reciver;
            }
        }
		
        public Message message
        {
            get
            {
                return pMessage;
            }
        }
    }
}


