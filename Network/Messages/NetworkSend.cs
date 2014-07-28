// Author: Piotr Trzpil

#region



#endregion

namespace Network.Messages
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using global::System;
    using global::System.Net;

    #endregion

    #region

    #endregion

    /// <summary>
    ///   Wylacznie obiekt tej klasy jest bezposrednio przesylany przez siec
    /// </summary>
    [Serializable]
    public class NetworkSend
    {
        private readonly Message pMessage;
        private readonly IPEndPoint pReciver;

        public NetworkSend(IPEndPoint reciver, Message message)
        {
            pReciver = reciver;
            pMessage = message;
        }

        public IPEndPoint reciver
        {
            get { return pReciver; }
        }

        public Message message
        {
            get { return pMessage; }
        }
    }
}