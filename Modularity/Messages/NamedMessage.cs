/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.Modularity.Messages
{
	#region
	using global::System;	
	#endregion
	
    [Serializable]
    public abstract class NamedMessage : Message
    {
        private readonly string pLogin;

        public NamedMessage(string login)
        {
            pLogin = login;
        }

        public string login
        {
            get { return pLogin; }
        }
    }
}