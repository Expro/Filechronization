/*
 * 
 * Author: Maciej Grabowski
 * 
 */

#region
using System;
using Filechronization.Modularity.Messages;
using Filechronization.Security;
#endregion

namespace Filechronization.Network.Tasks.Authorization.Messages
{
	[Serializable]
    public class SaltResponse : Message
    {
        private readonly Entropy pSalt;

        public SaltResponse(Entropy salt)
        {
            pSalt = salt;
        }

        public Entropy salt
        {
            get { return pSalt; }
        }
    }
}