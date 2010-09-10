/*
 * 
 * Author: Maciej Grabowski
 * 
 */

#region
using System;
using System.Net;
#endregion

namespace Filechronization.Network.Messages
{
	#region
	using Modularity.Messages;
	#endregion
	/// <summary>
	/// Wylacznie obiekt tej klasy jest bezposrednio przesylany przez siec
	/// </summary>
    [Serializable]
    public class NetworkSend
    {
        private IPEndPoint pReciver;
        private Message pMessage;
		
        public NetworkSend(IPEndPoint reciver, Message message)
        {
            pReciver = reciver;
            pMessage = message;
        }
		
        public IPEndPoint reciver
        {
            get
            {
                return pReciver;
            }
        }
		
        public Message message
        {
            get
            {
                return pMessage;
            }
        }
    }
}


