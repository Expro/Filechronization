/*
 * Author: Piotr Trzpil
 */

#region Usings
using Filechronization.Tasks.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Filechronization.Network.System.MainParts;
using Filechronization.Network.Messages;
using Filechronization.CommonClasses;
#endregion
 
namespace Filechronization.Network.System.Connections
{
    using ConsoleApplication1;
    using Modularity.Messages;

    public delegate void DisconnectionHandler(RemotePeer peer);

    /// <summary>
    ///   Obiekt enkapsulujacy polaczenie z innym komputerem
    /// </summary>
    public class RemotePeer : Peer
    {
        private readonly NetworksManager _manager;
        private readonly PeerProxy _proxy;

        
        private bool _persistent;

        
        private IPEndPoint _endPointAddress;

        public RemotePeer(NetworksManager manager, PeerProxy proxy)
        {
            _manager = manager;
            _proxy = proxy;
        }


        public override bool Connected
        {
            get
            {
                return !_proxy.IsDisposed;
            }
        }

        public override bool Persistent
        {
            get
            {
                return _proxy.Persistent;
            }
            set { _manager.SetPersistent(this, _proxy, value); }
        }

        /// <summary>
        ///   Zwraca endpoint skojarzony z polaczeniem
        /// </summary>
        public override IPEndPoint Endpoint
        {
            get
            {
                return _proxy.Endpoint;
            }
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
            _manager.HandleSend(_proxy, message);


        }

        
    }
}