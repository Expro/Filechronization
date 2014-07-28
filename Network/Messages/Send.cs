// Author: Piotr Trzpil
namespace Network.Messages
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using Filechronization.UserManagement;
    using global::System;

    #endregion

    #region

    #endregion

    [Serializable]
    public class Send : Message
    {
        private readonly User _reciver;
        private readonly Message pMessage;

        public Send(User reciver, Message message)
        {
            _reciver = reciver;
            pMessage = message;
        }

        public User reciver
        {
            get { return _reciver; }
        }

        public Message message
        {
            get { return pMessage; }
        }
    }
}