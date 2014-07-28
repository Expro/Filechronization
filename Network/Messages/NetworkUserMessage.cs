// Author: Piotr Trzpil
namespace Network.Messages
{
    #region Usings

    using global::System;

    #endregion

    #region Usings

    #endregion

    [Serializable]
    public abstract class NetworkUserMessage : ToStateMessage
    {
//         public readonly UserID Id;
//
//         protected NetworkUserMessage(UserID id)
//         {
//             Id = id;
//         }

        public readonly string Login;

        protected NetworkUserMessage(string login)
        {
            Login = login;
        }
    }
}