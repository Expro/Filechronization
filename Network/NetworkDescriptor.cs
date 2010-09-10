/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.Network
{
	#region
	using global::System;
	using global::System.Collections.Generic;
	//using Authorization;
	using Network.System.Connections;
	using CommonClasses;
	#endregion
	
    public class NetworkDescriptor
    {
        private string pName;
        private string pLogin;
        //private Password pPassword;
        private List<UnifiedAddress> pAddresses;
		
        public NetworkDescriptor(string name, string login, string password)
        {
            pName = name;
            pLogin = login;
            //pPassword = new Password(password);
            pAddresses = new List<UnifiedAddress>();
        }
		
        public void AddAddress(string IP, int port)
        {
            UnifiedAddress item = new UnifiedAddress(IP, port);
			
            if (!pAddresses.Contains(item))
                pAddresses.Add(item);
        }
		
        public string name
        {
            get
            {
                return pName;
            }
        }
		
        public string login
        {
            get
            {
                return pLogin;
            }
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
            get
            {
                return new List<UnifiedAddress>(pAddresses);
            }
        }
    }
}


