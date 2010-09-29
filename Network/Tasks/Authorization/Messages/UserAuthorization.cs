// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.Tasks.Authorization.Messages
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using Filechronization.Security;
    using global::System;

    #endregion

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