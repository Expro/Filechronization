// Author: Piotr Trzpil
namespace Network.Tasks.Authorization.Messages
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using global::System;

    #endregion

    #region

    #endregion

    [Serializable]
    public class AuthorizationRejected : NamedMessage
    {
        private readonly string pMessage;

        public AuthorizationRejected(string login, string message) : base(login)
        {
            pMessage = message;
        }

        public string message
        {
            get { return pMessage; }
        }
    }
}