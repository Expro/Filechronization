// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.Connections
{
    #region Usings

    using global::System;
    using global::System.IO;
    using global::System.Net.Sockets;
    using global::System.Runtime.Serialization.Formatters.Binary;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    #endregion

    #region Usings

    #endregion

    public class AsyncConnection //: IAsyncResult
    {
        private const int BufferSize = 5000;
        private readonly TcpClient _client;
        private readonly byte[] _dataBuffer;
        private readonly BinaryFormatter _inFormatter;
        private readonly object _locker;
        private readonly MemoryStream _memStream;
        private readonly NetworkStream _netStream;
        private readonly BinaryFormatter _outFormatter;
        private readonly MemoryStream _tmpStream;
        private readonly ManualResetEvent _waitHandle;

        private bool _alreadyStarted;
        private Action<PeerProxy> _callback;
        private int _dataLength = -1;
        private Exception _exception;


        private int _nextHeaderPos;
        private PeerProxy _proxy;

        public AsyncConnection(TcpClient client)
        {
            _client = client;

            _waitHandle = new ManualResetEvent(false);
            _dataBuffer = new byte[BufferSize];

            _netStream = _client.GetStream();
            _inFormatter = new BinaryFormatter();
            _outFormatter = new BinaryFormatter();
            _memStream = new MemoryStream(BufferSize);
            _tmpStream = new MemoryStream();

            _locker = new object();
        }

        public NetworkStream NetStream
        {
            get { return _netStream; }
        }

        public TcpClient Client
        {
            get { return _client; }
        }

        internal PeerProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }


        public void Send(object obj)
        {
            lock (_locker)
            {
                try
                {
                    _tmpStream.SetLength(0);

                    _outFormatter.Serialize(_tmpStream, obj);

                    byte[] header = NetHeader.CreateHeader(_tmpStream.Length);

                    _netStream.Write(header, 0, header.Length);
                    _netStream.Write(_tmpStream.GetBuffer(), 0, (int) _tmpStream.Length);
                }
                catch (IOException)
                {
                    throw;
                }

                catch (ObjectDisposedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Bug.Pr(e);
                }
            }
        }

        /// <summary>
        ///   Call once to start listening for incomming data.
        ///   Each deserialized object will be received after 
        ///   calling EndReceive in callback.
        /// </summary>
        public void BeginReceiving(Action<PeerProxy> callback)
        {
            lock (_locker)
            {
                if (_alreadyStarted)
                {
                    throw new InvalidOperationException("Method must be called only once.");
                }
                _alreadyStarted = true;
                _callback = callback;

                BeginNextPortion();
            }
        }

        private void BeginNextPortion()
        {
            _dataLength = -1;
            _waitHandle.Reset();

            Task.Factory.StartNew(ScheduleStreamRead)
                .ContinueWith(prev =>
                {
                    _exception = prev.Exception;
                    FireCallback();
                });
        }

        private void ScheduleStreamRead()
        {
            Task<int> netStreamRead = Task<int>.Factory.FromAsync(
                _netStream.BeginRead, _netStream.EndRead,
                _dataBuffer, 0, BufferSize, null, TaskCreationOptions.AttachedToParent);

            netStreamRead.ContinueWith(AfterStreamRead,
                                       TaskContinuationOptions.NotOnFaulted
                                       | TaskContinuationOptions.AttachedToParent
                );
        }

        private void AfterStreamRead(Task<int> netStreamRead)
        {
            if (netStreamRead.Result > 0)
            {
                _memStream.Write(_dataBuffer, 0, netStreamRead.Result);

                if (_dataLength < 0 && _memStream.Length >= NetHeader.HeaderLength)
                {
                    _dataLength = NetHeader.ReadLengthFromHeader(_memStream, 0);
                }
                // czy to jeszcze nie wszystkie dane?
                if (_dataLength < 0 || (_dataLength >= 0 && _memStream.Length - NetHeader.HeaderLength < _dataLength))
                {
                    ScheduleStreamRead();
                }
            }
        }

        private void FireCallback()
        {
            try
            {
                _waitHandle.Set();
                if (_callback != null)
                    _callback(_proxy);
            }
            catch (Exception e)
            {
                Bug.Pr(e);
            }
        }

        /// <summary>
        ///   Must be called from a callback, returns deserialized object.
        ///   All exceptions must be cautch, otherwise UnobservedTaskException 
        ///   will be thrown.
        /// </summary>
        /// <returns>Object deserialized from NetworkStream</returns>
        public object EndReceive()
        {
            _waitHandle.WaitOne();

            if (_exception != null)
            {
                throw _exception;
            }

            _memStream.Position = _nextHeaderPos + NetHeader.HeaderLength;
            object received = _inFormatter.Deserialize(_memStream);

            _nextHeaderPos += NetHeader.HeaderLength + _dataLength;


            if (NetHeader.IsFullObjectInStream(_memStream, _nextHeaderPos, out _dataLength))
            {
                Task.Factory.StartNew(FireCallback);
            }
            else
            {
                TrimBeginning(_memStream);

                _nextHeaderPos = 0;
                BeginNextPortion();
            }


            return received;
        }


        private static void TrimBeginning(MemoryStream stream)
        {
            // skopiowanie pozostalych bajtow na poczatek strumienia
            int otherBytesLength = (int) (stream.Length - stream.Position);
            byte[] otherBytes = new byte[otherBytesLength];
            stream.Read(otherBytes, 0, otherBytesLength);
            stream.SetLength(otherBytesLength);
            stream.Position = 0;
            stream.Write(otherBytes, 0, otherBytesLength);
        }
    }
}