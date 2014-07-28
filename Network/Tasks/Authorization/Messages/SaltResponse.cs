// Author: Piotr Trzpil

#region



#endregion

namespace Network.Tasks.Authorization.Messages
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using Filechronization.Security;
    using global::System;

    #endregion

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