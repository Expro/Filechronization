namespace ConsoleApplication1
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Filechronization.Modularity.Messages;
    using Filechronization.Network.Messages;
    using Filechronization.Network.System.Connections;
    using Filechronization.Network.System.MainParts;

    public class NetworksManager
    {

        

        private ConnectionManager _manager;
        private Dictionary<NetworkID, PeerCenter> _registeredNetworks;

        private MultiPeerMap _multiPeerMap;

        public NetworksManager(ConnectionManager manager)
        {
            _manager = manager;
            _registeredNetworks = new Dictionary<NetworkID, PeerCenter>();


            
            _multiPeerMap = new MultiPeerMap(this);

            

            _manager.ObjectReceived += ObjectReceived;
       
            _manager.ConnectionClosed += ConnectionClosed;
        }
        private void ConnectionClosed(PeerProxy proxy)
        {
            

            _multiPeerMap.ForEachNetwork(proxy, 
                (net, peer) => net.ConnectionLost(peer));

            _multiPeerMap.Remove(proxy);
        }
        public void SetPersistent(RemotePeer remotePeer, PeerProxy proxy, bool value)
        {
            if(value)
            {
                proxy.Persistent = true;
            }
//            proxy.Persistent = _multiPeerMap.SetPersistent(remotePeer, proxy, value);
        }

        private void ObjectReceived(PeerProxy proxy, object obj)
        {
            NetworkSend netObject = obj as NetworkSend;
            if (netObject != null)
            {
                bool createdMapping;
                PeerCenter net = SelectNetwork(netObject);
                RemotePeer peer = _multiPeerMap.GetOrCreatePeer(net, proxy);

//                if (createdMapping)
//                {
//                    net.ConnectionAccepted(peer);
//                }
                
                net.ObjectReceived(peer, netObject.message);
            }
            else
            {
                proxy.Dispose();
            }
        }

        public void RegisterNetwork(PeerCenter net)
        {
            _registeredNetworks.Add(null, net);
        }

        

        private PeerCenter SelectNetwork(NetworkSend obj)
        {
            NetworkID id = ReadNetworkId(obj);
            try
            {
                return _registeredNetworks[id];
            }
            catch (ArgumentException)
            {
                throw new UnknownNetworkException();
            }
            
        }
        private NetworkID ReadNetworkId(NetworkSend obj)
        {
            throw new NotImplementedException();
            throw new UnknownNetworkException();
        }
        public RemotePeer Connect(PeerCenter net,IPAddress address)
        {
            return _multiPeerMap.GetOrCreatePeer(net, _manager.InstantConnect(address));
        }
        public Task<RemotePeer> ConnectTask(PeerCenter net, IPAddress address)
        {
            return _manager.ConnectTask(address)
                .ContinueWith(prev => _multiPeerMap.GetOrCreatePeer(net, prev.Result));
        }
        
        
        public void HandleSend(PeerProxy proxy, Message message)
        {
            NetworkSend send = PackMessage(message);
            proxy.Send(send);
        }

        private NetworkSend PackMessage(Message message)
        {
            throw new NotImplementedException();
        }



        
        private class NetworkID
        {
        }

        
    }

    internal class UnknownNetworkException : Exception
    {
    }
}