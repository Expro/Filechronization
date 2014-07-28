// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.Connections
{
    #region Usings

    using System.Connections;
    using System.MainParts;
    using global::System;
    using global::System.Collections.Generic;

    #endregion

    /// <summary>
    ///   Matches connections to multiple network handles
    /// </summary>
    public class MultiPeerMap
    {
        private readonly Dictionary<PeerProxy, ConnProperties> _connections;
        private readonly ConnectionManagerHigher _managerHigher;

        public MultiPeerMap(ConnectionManagerHigher managerHigher)
        {
            _managerHigher = managerHigher;
            _connections = new Dictionary<PeerProxy, ConnProperties>();
        }

        public RemotePeer GetOrCreatePeer(NetworkManager net, PeerProxy proxy)
        {
            RemotePeer retValue;
            lock (_connections)
            {
                ConnProperties prop;
                if (_connections.TryGetValue(proxy, out prop))
                {
                    // polaczenie juz istnieje
                    retValue = prop.GetOrRegister(net);
                }
                else
                {
                    // polaczenie jeszcze nie wpisane do mapy
                    prop = new ConnProperties(_managerHigher, proxy);
                    retValue = prop.GetOrRegister(net);
                    _connections.Add(proxy, prop);
                }
            }

            return retValue;
        }

        public void ForEachNetwork(PeerProxy proxy, Action<NetworkManager, RemotePeer> action)
        {
            List<KeyValuePair<NetworkManager, RemotePeer>> tmpList = new List<KeyValuePair<NetworkManager, RemotePeer>>();

            lock (_connections) tmpList.AddRange(_connections[proxy].Networks);

            foreach (KeyValuePair<NetworkManager, RemotePeer> pair in tmpList)
            {
                action(pair.Key, pair.Value);
            }
        }

        public void Remove(PeerProxy proxy)
        {
            lock (_connections)
            {
                _connections[proxy].Networks.Clear();
                _connections.Remove(proxy);
            }
        }

        #region Nested type: ConnProperties

        private class ConnProperties
        {
            private readonly ConnectionManagerHigher _managerHigher;
            private readonly Dictionary<NetworkManager, RemotePeer> _owners;
            private readonly PeerProxy _proxy;
            private Dictionary<RemotePeer, bool> _persistent;

            public ConnProperties(ConnectionManagerHigher managerHigher, PeerProxy proxy)
            {
                _managerHigher = managerHigher;
                _proxy = proxy;
                _owners = new Dictionary<NetworkManager, RemotePeer>();
            }

            public int PersistentCount { get; set; }

            public Dictionary<RemotePeer, bool> Persistent
            {
                get { return _persistent; }
            }

            public Dictionary<NetworkManager, RemotePeer> Networks
            {
                get { return _owners; }
            }


            public RemotePeer GetOrRegister(NetworkManager net)
            {
                RemotePeer peer;
                if (_owners.TryGetValue(net, out peer))
                {
                    // polaczenie juz zwiazane z tym managerem
                    return peer;
                }
                // polaczenie nie zwiazane z tym managerem
                peer = new RemotePeer(_managerHigher, _proxy);
                _owners.Add(net, peer);
                return peer;
            }
        }

        #endregion
    }
}