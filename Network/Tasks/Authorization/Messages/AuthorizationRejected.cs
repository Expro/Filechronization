/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.Network.Tasks.Authorization.Messages
{
	#region
	using global::System;
	using Modularity.Messages;
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