// Author: Piotr Trzpil

namespace Network.Connections
{
    #region Usings

    using System.Connections;
    using System.MainParts;
    using Filechronization.Modularity.Messages;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Threading.Tasks;
    using Messages;

    #endregion

    public class NetworksManager
    {
        private readonly ConnectionManager _manager;

        private readonly MultiPeerMap _multiPeerMap;
        private readonly Dictionary<NetworkID, PeerCenter> _registeredNetworks;

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
            if (value)
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
            catch (KeyNotFoundException)
            {
                throw new UnknownNetworkException();
            }
        }

        private NetworkID ReadNetworkId(NetworkSend obj)
        {
            throw new NotImplementedException();
            throw new UnknownNetworkException();
        }

        public RemotePeer Connect(PeerCenter net, IPAddress address)
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

        #region Nested type: NetworkID

        private class NetworkID
        {
        }

        #endregion
    }

    internal class UnknownNetworkException : Exception
    {
    }
}