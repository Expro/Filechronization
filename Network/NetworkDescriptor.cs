// Author: Piotr Trzpil
namespace Network
{
    #region Usings

    using Filechronization.CommonClasses;
    using global::System.Collections.Generic;

    #endregion

    #region

    //using Authorization;

    #endregion

    public class NetworkDescriptor
    {
        //private Password pPassword;
        private readonly List<UnifiedAddress> pAddresses;
        private readonly string pLogin;
        private readonly string pName;

        public NetworkDescriptor(string name, string login, string password)
        {
            pName = name;
            pLogin = login;
            //pPassword = new Password(password);
            pAddresses = new List<UnifiedAddress>();
        }

        public string name
        {
            get { return pName; }
        }

        public string login
        {
            get { return pLogin; }
        }

//        public Password password
//        {
//            get
//            {
//                return pPassword;
//            }
//        }

        public List<UnifiedAddress> addresses
        {
            get { return new List<UnifiedAddress>(pAddresses); }
        }

        public void AddAddress(string IP, int port)
        {
            UnifiedAddress item = new UnifiedAddress(IP, port);

            if (!pAddresses.Contains(item))
                pAddresses.Add(item);
        }
    }
}