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
        private readonly NetworksManager _manager;

        public MultiPeerMap(NetworksManager manager)
        {
            _manager = manager;
            _connections = new Dictionary<PeerProxy, ConnProperties>();
        }

        public RemotePeer GetOrCreatePeer(PeerCenter net, PeerProxy proxy)
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
                    prop = new ConnProperties(_manager, proxy);
                    retValue = prop.GetOrRegister(net);
                    _connections.Add(proxy, prop);
                }
            }

            return retValue;
        }

        public void ForEachNetwork(PeerProxy proxy, Action<PeerCenter, RemotePeer> action)
        {
            List<KeyValuePair<PeerCenter, RemotePeer>> tmpList = new List<KeyValuePair<PeerCenter, RemotePeer>>();

            lock (_connections) tmpList.AddRange(_connections[proxy].Networks);

            foreach (KeyValuePair<PeerCenter, RemotePeer> pair in tmpList)
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
            private readonly NetworksManager _manager;
            private readonly Dictionary<PeerCenter, RemotePeer> _owners;
            private readonly PeerProxy _proxy;
            private Dictionary<RemotePeer, bool> _persistent;

            public ConnProperties(NetworksManager manager, PeerProxy proxy)
            {
                _manager = manager;
                _proxy = proxy;
                _owners = new Dictionary<PeerCenter, RemotePeer>();
            }

            public int PersistentCount { get; set; }

            public Dictionary<RemotePeer, bool> Persistent
            {
                get { return _persistent; }
            }

            public Dictionary<PeerCenter, RemotePeer> Networks
            {
                get { return _owners; }
            }


            public RemotePeer GetOrRegister(PeerCenter net)
            {
                RemotePeer peer;
                if (_owners.TryGetValue(net, out peer))
                {
                    // polaczenie juz zwiazane z tym managerem
                    return peer;
                }
                // polaczenie nie zwiazane z tym managerem
                peer = new RemotePeer(_manager, _proxy);
                _owners.Add(net, peer);
                return peer;
            }
        }

        #endregion
    }
}