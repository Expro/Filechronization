/*
 * 
 * Author: Maciej Grabowski
 * 
 */

#region Usings
using System;
using Filechronization.Security;
using Filechronization.Modularity.Messages;
#endregion

namespace Filechronization.Network.Tasks.Authorization.Messages
{
	[Serializable]
    public class UserAuthorization : NamedMessage
    {
        private readonly Entropy pSaltedPassword;

        public UserAuthorization(string login, Entropy saltedPassword) : base(login)
        {
            pSaltedPassword = saltedPassword;
        }

        public Entropy saltedPassword
        {
            get { return pSaltedPassword; }
        }
    }
}