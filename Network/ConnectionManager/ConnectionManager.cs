namespace ConsoleApplication1
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    ///   Global manager for connections. Handles connections lifes
    ///   and sending and receiving of objects.
    /// </summary>
    public class ConnectionManager
    {
        private const int PortNumber = 49334;
        private readonly Thread _connectThread;

        private readonly TcpListener _listener;

        private Dictionary<IPEndPoint, AsyncConnection> _connections;


        private Dictionary<IPAddress, TcpClient> _unfinishedConnections;

        public event Action<PeerProxy,object> ObjectReceived;
        public event Action<PeerProxy> ConnectionClosed;
        public event Action<PeerProxy> ConnectionAccepted;



        

        public ConnectionManager()
        {
            _listener = new TcpListener(IPAddress.Any, PortNumber);
            _connectThread = new Thread(ListenForConnections);
            _connections = new Dictionary<IPEndPoint, AsyncConnection>();
            _unfinishedConnections = new Dictionary<IPAddress, TcpClient>();
            
        }

        

        public void StartServer()
        {
            _connectThread.Start();
        }

        private void ListenForConnections()
        {
            _listener.Start();

            while (true)
            {
                try
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    IPEndPoint endpoint = (IPEndPoint) client.Client.LocalEndPoint;
                    Console.WriteLine("Accepted from: " + endpoint);
                    AsyncConnection conn = new AsyncConnection(client);

                    lock (_connections)
                    {
                        _connections.Add(endpoint, conn);
                    }
                    PeerProxy proxy = CreateProxy();
                    proxy.Connection = conn;
                    conn.Proxy = proxy;

                    if (ConnectionAccepted != null)
                    {
                        ConnectionAccepted(proxy);
                    }

                    proxy.TryRestartTimer();

                    conn.BeginReceiving(ReceivingCallback);
                }
                catch (Exception e)
                {
                    Bug.Pr(e);
                }
            }
        }

        public PeerProxy InstantConnect(IPAddress address)
        {
            return BeginConnect(address, Callback, null).Proxy;
        }
        private void Callback(IAsyncResult result)
        {
            try
            {
                EndConnect(result);
            }
            catch (ConnectionClosedException)
            {
                
            }
            catch (CannotConnectException)
            {
                
            }
        }
        public ProxyAsyncState BeginConnect(IPAddress address, AsyncCallback callback, object state)
        {
            AsyncConnection conn;

            lock (_connections)
                if (_connections.TryGetValue(new IPEndPoint(address, PortNumber), out conn))
                {
                    return new ProxyAsyncState(conn.Proxy, state);
                }

            TcpClient client = new TcpClient();

            try
            {
                lock (_unfinishedConnections)
                    _unfinishedConnections.Add(address, client);
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException("Connection is under way.");
            }


            PeerProxy proxy = CreateProxy();
            try
            {
                client.BeginConnect(address, PortNumber, callback,
                                    Tuple.Create(new ConnectionAsyncResult(client, proxy, state), address));
            }
            catch (Exception)
            {
                lock (_unfinishedConnections)
                    _unfinishedConnections.Remove(address);

                throw new CannotConnectException();
            }

            return new ProxyAsyncState(proxy, state);
        }


        public PeerProxy EndConnect(IAsyncResult result)
        {
            Tuple<ConnectionAsyncResult, IPAddress> tuple = (Tuple<ConnectionAsyncResult, IPAddress>) result.AsyncState;
            ConnectionAsyncResult connResult = tuple.Item1;
            IPAddress address = tuple.Item2;
            try
            {
                connResult.Client.EndConnect(result);

                AsyncConnection connection = new AsyncConnection(connResult.Client);
                
                lock (_connections)
                    _connections.Add((IPEndPoint) connResult.Client.Client.LocalEndPoint, connection);

                lock (_unfinishedConnections)
                    _unfinishedConnections.Remove(address);

                connection.Proxy = connResult.Proxy;
                connResult.Proxy.Connection = connection;
                connection.Proxy.TryRestartTimer();

                connection.BeginReceiving(ReceivingCallback);
            }
            catch (ConnectionClosedException)
            {
                HandleDisconnect(connResult.Proxy, true);
                // moze wystapic przy wysylaniu wiadomosci z kolejki
                // w connResult.Proxy.Connection = connection;
                throw;
            }
            catch (SocketException)
            {
                HandleDisconnect(connResult.Proxy, true);
                throw new CannotConnectException();
            }
            catch (ObjectDisposedException)
            {
                HandleDisconnect(connResult.Proxy, true);
                throw new CannotConnectException();
            }
            catch (Exception e)
            {
                HandleDisconnect(connResult.Proxy, true);
                Bug.Pr(e);
            }
            return connResult.Proxy;
        }


        private PeerProxy CreateProxy()
        {
            return new PeerProxy(this);
        }


        private void ReceivingCallback(PeerProxy proxy)
        {
            try
            {
                object obj = proxy.Connection.EndReceive();
                proxy.TryRestartTimer();

                if (ObjectReceived != null)
                {
                    ObjectReceived(proxy, obj);
                }
            }
            catch (Exception)
            {
                HandleDisconnect(proxy, true);
                //   throw new ConnectionClosedException();
            }
        }


        private void HandleDisconnect(PeerProxy proxy, bool fireEvent)
        {
            Console.WriteLine("HandleDisconnect");
            try
            {
                lock (_connections)
                {
                    AsyncConnection connection = proxy.Connection;
                    IPEndPoint endpoint = (IPEndPoint) connection.Client.Client.LocalEndPoint;
                    _connections.Remove(endpoint);

                    proxy.Dispose();

                    if (fireEvent && ConnectionClosed != null)
                    {
                        ConnectionClosed(proxy);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Socket jest juz zamkniety
            }
        }
        public Task<PeerProxy> ConnectTask(IPAddress address)
        {
            return Task<PeerProxy>.Factory.FromAsync(BeginConnect, EndConnect, address, null);
        }

        

        
    }

   // public delegate void AcceptedEventHandler(object sender, PeerProxy proxy);

    public class ProxyAsyncState : IAsyncResult
    {
        private readonly PeerProxy _peerProxy;
        private readonly object _state;
        private WaitHandle _waitHandle;

        public ProxyAsyncState(PeerProxy peerProxy, object state)
        {
            _peerProxy = peerProxy;
            _state = state;
            _waitHandle = new ManualResetEvent(false);
        }

        #region Implementation of IAsyncResult

        public bool IsCompleted
        {
            get { return _waitHandle.WaitOne(0, false); }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { return _waitHandle; }
        }

        public object AsyncState
        {
            get { return _state; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        #endregion

        public PeerProxy Proxy
        {
            get { return _peerProxy; }
        }
    }

    public class ConnectionAsyncResult : ProxyAsyncState
    {
        private readonly TcpClient _client;


        public ConnectionAsyncResult(TcpClient client, PeerProxy proxy, object state) : base(proxy, state)
        {
            _client = client;
        }
        
        public TcpClient Client
        {
            get { return _client; }
        }
    }
}