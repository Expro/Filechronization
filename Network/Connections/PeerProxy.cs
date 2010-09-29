// Author: Piotr Trzpil

#region Usings

#endregion

namespace Network.Connections
{
    #region Usings

    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Runtime.Serialization;
    using global::System.Threading;

    #endregion

    public class PeerProxy : IDisposable
    {
        private const int DisconnectTimeout = 30000;
        private readonly Timer _disconnectTimer;
        private readonly object _locker;
        private readonly ConnectionManager _manager;
        private AsyncConnection _connection;


        private bool _disposed;
        private IPEndPoint _endpoint;
        private Queue<object> _messageQueue;
        private bool _persistent;

        internal PeerProxy(ConnectionManager manager)
        {
            _manager = manager;
            _locker = new object();
            _messageQueue = new Queue<object>();
            _disconnectTimer = new Timer(TimerTick, null, DisconnectTimeout, Timeout.Infinite);
        }

        public bool IsDisposed
        {
            get
            {
                lock (_locker)
                    return _disposed;
            }
        }

        public bool Persistent
        {
            get
            {
                lock (_locker)
                    return _persistent;
            }
            set
            {
                lock (_locker)
                {
                    _persistent = value;

                    int timeout = value ? Timeout.Infinite : DisconnectTimeout;
                    _disconnectTimer.Change(timeout, Timeout.Infinite);
                }
            }
        }

        internal AsyncConnection Connection
        {
            get
            {
                lock (_locker)
                {
                    CheckDisposed();
                    return _connection;
                }
            }
            set
            {
                lock (_locker)
                {
                    CheckDisposed();
                    if (_connection == null)
                    {
                        _connection = value;
                        _endpoint = (IPEndPoint) _connection.Client.Client.RemoteEndPoint;
                        while (_messageQueue.Count > 0)
                        {
                            Send(_messageQueue.Dequeue());
                        }
                        _messageQueue = null;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        public IPEndPoint Endpoint
        {
            get { return _endpoint; }
        }

        private void TimerTick(object state)
        {
            lock (_locker)
            {
                if (!_persistent)
                {
                    Dispose();
                }
            }
        }

        public void Send(object obj)
        {
            lock (_locker)
            {
                CheckDisposed();
                if (_connection != null)
                {
                    try
                    {
                        _connection.Send(obj);
                        TryRestartTimer();
                        //_manager.AcknowledgeTraffic(this);
                    }
                    catch (SerializationException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        //_manager.HandleDisconnect(this);
                        throw new ConnectionClosedException();
                    }
                }
                else
                {
                    _messageQueue.Enqueue(obj);
                }
            }
        }

        private void CheckDisposed()
        {
            lock (_locker)
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("Connection object is disposed.");
                }
            }
        }

        internal bool TryRestartTimer()
        {
            lock (_locker)
            {
                if (!Persistent)
                {
                    try
                    {
                        _disconnectTimer.Change(DisconnectTimeout, Timeout.Infinite);
                        return true;
                    }
                    catch (ObjectDisposedException)
                    {
                        // timer jest Disposed, nic nie rob
                    }
                }
                return false;
            }
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            lock (_locker)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    Connection.NetStream.Close();
                    Connection.Client.Close();
                    _disconnectTimer.Dispose();
                }
            }
        }

        #endregion
    }
}