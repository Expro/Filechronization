/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.Network.Tasks.Authorization.Messages
{
	#region
	using global::System;
	using global::System.Collections.Generic;
	using global::System.Net;
	using Modularity.Messages;
	#endregion
	
    [Serializable]
    public class AuthorizationAccepted : NamedMessage
    {
        public Dictionary<string, IPAddress> UserAddresses
        {
            get;
            set;
        }

        public readonly string ArbiterLogin;
        public AuthorizationAccepted(string login, string arbiterLogin, Dictionary<string, IPAddress> users)
            : base(login)
        {
            UserAddresses = users;
            ArbiterLogin = arbiterLogin;
        }
    }
}