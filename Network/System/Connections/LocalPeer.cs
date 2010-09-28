/*
 * Author: Piotr Trzpil
 */

#region Usings
using Filechronization.Tasks.Messages;
using System;
using System.Net;
using Filechronization.Network.System.MainParts;
using Filechronization.Network.Messages;
#endregion
    
namespace Filechronization.Network.System.Connections
{
    using Modularity.Messages;

    // Loopback na siebie
    public class LocalPeer : Peer
    {
        private readonly NetworkModule _netModule;

        
        public LocalPeer(NetworkModule netModule)
        {
            _netModule = netModule;
        }

        public override bool Connected
        {
            get { return true; }
        }

        public override bool Persistent
        {
            get { return true; }
            set { throw new InvalidOperationException(); }
        }

        public override IPEndPoint Endpoint
        {
            get { return new IPEndPoint(IPAddress.Loopback, 0); }
        }

        

        public override void Send(Message netMessage)
        {

            _netModule.netQueue.Add(() =>
                _netModule.PeerCenter.ObjectReceived(this, netMessage));
        }


    }
}