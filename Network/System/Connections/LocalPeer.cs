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

    // Loopback na siebie
    public class LocalPeer : Peer
    {
        private readonly NetworkModule _netModule;

        // private IPAddress _address;


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

        public override IPEndPoint EndPointAddress
        {
            get { return new IPEndPoint(IPAddress.Loopback, 0); }
        }

        public override void Disconnect()
        {
        }


        public override void Send(NetworkSend netMessage)
        {
            if (netMessage.message is TaskMessage)
            {
                var task = (TaskMessage) netMessage.message;
            }
            else
            {

            }


            _netModule.netQueue.Add(
                delegate { OnObjectReceived(netMessage); });
        }

//        public override void StopTimer()
//        {
//        }

        public override void TryRestartTimer()
        {
        }

        public override void AfterConnect(AfterConnectExec exec)
        {
            exec();
        }
    }
}