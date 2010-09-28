namespace Filechronization.Network
{
    #region Usings

    using System.Connections;
    using System.MainParts;
    using global::System;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Runtime.Serialization.Formatters.Binary;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    #endregion

    /// <summary>
    /// Global manager for connections. Handles connections lifes
    /// and sending and receiving of objects.
    /// </summary>
    public class ConnectionManager
    {
        private readonly Thread _connectThread;

        private readonly TcpListener _listener;

        private BinaryFormatter _inFormatter;
        private BinaryFormatter _outFormatter;

        public ConnectionManager()
        {
            
        }

        public void ListenForConnections()
        {
            _listener.Start();

            while (true)
            {
                try
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    
                    
                        ConnectionAccepted(client);
                }
                catch (SocketException e)
                {
                    P.pr(e);
                }
            }
        }

        public void BeginConnect()
        {
            RemotePeer peer = null;
            if (_connections.TryGetValue(new IPEndPoint(address, NetworkModule.portNr), out peer))
            {
                return peer;
            }


            if (!_unfinishedConnections.TryGetValue(address, out peer))
            {
                peer = new RemotePeer();
                try
                {
                    peer.BeginConnectAsync(address, address, PeerConnectionResult);
                    _unfinishedConnections.Add(address, peer);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
            else
            {
                //NetworkModule.SendNotification("Possible bug in BeginConnect " + address);
            }


            return peer;
        }


        public class MessagingServices
        {
            public static IAsyncResult BeginReverseEcho(TcpClient client,
                                                        AsyncCallback callback,
                                                        object userState)
            {
                ReverseEcho re = new ReverseEcho();
                re.Begin(client, callback, userState);
                return re;
            }

            public static byte[] EndReverseEcho(IAsyncResult r)
            {
                return ((ReverseEcho) r).End();
            }
        }

        private class ReverseEcho : IAsyncResult
        {
            private TcpClient _client;
            private NetworkStream _stream;
            private object _userState;
            private readonly ManualResetEvent _waitHandle = new ManualResetEvent(false);
            private int _bytesRead;
            private readonly byte[] _data = new byte[5000];
            private Exception _exception;
            // IAsyncResult members:
            public object AsyncState
            {
                get { return _userState; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get { return _waitHandle; }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return _waitHandle.WaitOne(0, false); }
            }

            internal void Begin(TcpClient c, AsyncCallback callback, object state)
            {
                _client = c;
                _userState = state;
                _stream = _client.GetStream();
                Task.Factory.StartNew(Read).ContinueWith(ant =>
                    {
                        _exception = ant.Exception;
                        // In case an exception occurred.
                        if (_stream != null)
                            try
                            {
                                _stream.Close();
                            }
                            catch (Exception ex)
                            {
                                _exception = ex;
                            }
                        ;
                        _waitHandle.Set();
                        if (callback != null) callback(this);
                    });
            }

            internal byte[] End() // Wait for completion + rethrow any error.
            {
                AsyncWaitHandle.WaitOne();
                if (_exception != null)
                    throw _exception;
                return _data;
            }

            private void Read()
            {
                Task<int> readChunk = Task<int>.Factory.FromAsync(
                    _stream.BeginRead, _stream.EndRead,
                    _data, 0, _data.Length - _bytesRead, null);
                readChunk.ContinueWith(ContinueRead,
                                       TaskContinuationOptions.NotOnFaulted);
            }

            private void ContinueRead(Task<int> readChunk)
            {
                _bytesRead += readChunk.Result;
                if (readChunk.Result > 0 && _bytesRead < _data.Length)
                {
                    Read(); // More data to read!
                    return;
                }
                Array.Reverse(_data);
                Task.Factory.FromAsync(_stream.BeginWrite, _stream.EndWrite,
                                       _data, 0, _data.Length, null);
            }
        }
    }
}