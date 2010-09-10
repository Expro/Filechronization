/*
 * Author: Piotr Trzpil
 */

namespace Filechronization.Network.Messages
{
    #region Usings

    using global::System;
    using global::System.Net;

    #endregion

    public enum UserState
    {
        ONLINE,
        OFFLINE
    }

    [Serializable]
    public class UserStateChanged : NetworkUserMessage
    {
        public readonly UserState State;
        public readonly IPEndPoint Address;

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