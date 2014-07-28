// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.System.Connections
{
    #region Usings

    using Filechronization.CommonClasses;
    using Filechronization.Modularity.Messages;
    using global::System;
    using global::System.Net;
    using Network.Connections;

    #endregion

    public delegate void DisconnectionHandler(RemotePeer peer);

    /// <summary>
    ///   Obiekt enkapsulujacy polaczenie z innym komputerem
    /// </summary>
    public class RemotePeer : Peer
    {
        private readonly ConnectionManagerHigher _managerHigher;
        private readonly PeerProxy _proxy;


        private IPEndPoint _endPointAddress;
        private bool _persistent;

        public RemotePeer(ConnectionManagerHigher managerHigher, PeerProxy proxy)
        {
            _managerHigher = managerHigher;
            _proxy = proxy;
        }


        public override bool Connected
        {
            get { return !_proxy.IsDisposed; }
        }

        public override bool Persistent
        {
            get { return _proxy.Persistent; }
            set { _managerHigher.SetPersistent(this, _proxy, value); }
        }

        /// <summary>
        ///   Zwraca endpoint skojarzony z polaczeniem
        /// </summary>
        public override IPEndPoint Endpoint
        {
            get { return _proxy.Endpoint; }
        }


        /// <summary>
        ///   Tlumaczy obiekt na adres ip jesli jest to mozliwe
        /// </summary>
        /// <param name = "address"></param>
        /// <returns></returns>
        public static IPAddress ToIpAddress(object address)
        {
            if (address is IPAddress)
            {
                return (IPAddress) address;
            }
            if (address is UnifiedAddress)
            {
                return ExtractAddress((UnifiedAddress) address);
            }
            return null;
        }


        public static IPAddress ExtractAddress(UnifiedAddress dnsOrIP)
        {
            try
            {
                return IPAddress.Parse(dnsOrIP.address);
            }
            catch (FormatException)
            {
                return Dns.GetHostEntry(dnsOrIP.address).AddressList[0];
            }
        }


        public override void Send(Message message)
        {
            _managerHigher.HandleSend(_proxy, message);
        }
    }
}