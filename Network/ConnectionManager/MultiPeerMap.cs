namespace ConsoleApplication1
{
    using System;
    using System.Collections.Generic;
    using Filechronization.Network.System.Connections;
    using Filechronization.Network.System.MainParts;

    public class MultiPeerMap
    {
        private readonly NetworksManager _manager;
        private Dictionary<PeerProxy, ConnProperties> _connections;

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
            var tmpList = new List<KeyValuePair<PeerCenter, RemotePeer>>();

            lock (_connections) tmpList.AddRange(_connections[proxy].Networks);

            foreach (var pair in tmpList)
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
//        public bool SetPersistent(RemotePeer peer, PeerProxy proxy, bool value)
//        {
//            var prop = _connections[proxy];
//            if(prop.Persistent[peer])
//        }

        private class ConnProperties
        {


            private Dictionary<PeerCenter, RemotePeer> _owners;
            private Dictionary<RemotePeer, bool> _persistent;
            private readonly NetworksManager _manager;
            private PeerProxy _proxy;
            public int PersistentCount
            {
                get;
                set;
            }

            public Dictionary<RemotePeer, bool> Persistent
            {
                get { return _persistent; }
            }

            public ConnProperties(NetworksManager manager, PeerProxy proxy)
            {
                _manager = manager;
                _proxy = proxy;
                _owners = new Dictionary<PeerCenter, RemotePeer>();

            }

            public Dictionary<PeerCenter, RemotePeer> Networks
            {
                get
                {
                    return _owners;
                }
               
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


        
    }
}