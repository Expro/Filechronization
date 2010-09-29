// Author: Piotr Trzpil
namespace Network.Messages
{
    #region Usings

    using global::System;
    using global::System.Net;

    #endregion

    #region Usings

    #endregion

    public enum UserState
    {
        ONLINE,
        OFFLINE
    }

    [Serializable]
    public class UserStateChanged : NetworkUserMessage
    {
        public readonly IPEndPoint Address;
        public readonly UserState State;

//        public UserStateChanged(UserID id, UserState state)
//            :base(id)
//        {
//            State = state;
//        }

        public UserStateChanged(string login, UserState state, IPEndPoint address)
            : base(login)
        {
            State = state;
            Address = address;
        }
    }
}