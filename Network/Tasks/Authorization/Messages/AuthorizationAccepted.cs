// Author: Piotr Trzpil
namespace Network.Tasks.Authorization.Messages
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;

    #endregion

    #region

    #endregion

    [Serializable]
    public class AuthorizationAccepted : NamedMessage
    {
        public readonly string ArbiterLogin;

        public AuthorizationAccepted(string login, string arbiterLogin, Dictionary<string, IPAddress> users)
            : base(login)
        {
            UserAddresses = users;
            ArbiterLogin = arbiterLogin;
        }

        public Dictionary<string, IPAddress> UserAddresses { get; set; }
    }
}