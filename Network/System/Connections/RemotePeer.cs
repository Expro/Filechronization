/*
 * Author: Piotr Trzpil
 */

#region Usings
using Filechronization.Tasks.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Filechronization.Network.System.MainParts;
using Filechronization.Network.Messages;
using Filechronization.CommonClasses;
#endregion
 
namespace Filechronization.Network.System.Connections
{
	public delegate void DisconnectionHandler(RemotePeer peer);

    /// <summary>
    ///   Obiekt enkapsulujacy polaczenie z innym komputerem
    /// </summary>
    public class RemotePeer : Peer
    {
        public static int maxIdleTime = 10000;

        private readonly TcpClient _client;
        private BinaryFormatter _inFormatter;
        private BinaryFormatter _outFormatter;
        private bool _connected;
        private bool _persistent;

        private NetworkStream _stream;
        private IPEndPoint _endPointAddress;

        // timer rozlaczajacy polaczenie po okreslonym czasie nieaktywnosci
        private readonly Timer _disconnectTimer;


        private Thread _listeningThread;

        /// <summary>
        ///  kolejka oczekujacych (przed ustanowieniem polaczenia) na wyslanie wiadomosci 
        /// </summary>
        private readonly Queue<NetworkSend> _messageQueue;


        /// <summary>
        ///  kolejka oczekujacych  (przed ustanowieniem polaczenia) na wykonanie operacji
        /// </summary>
        private readonly Queue<AfterConnectExec> _execQueue;


        public event DisconnectionHandler ConnectionLost;


        public override bool Connected
        {
            get { return _connected; }
        }

        public override bool Persistent
        {
            get { return _persistent; }
            set
            {
                _persistent = value;

                int timeout = value ? Timeout.Infinite : maxIdleTime;
                _disconnectTimer.Change(timeout, Timeout.Infinite);
            }
        }

        /// <summary>
        ///   Zwraca endpoint skojarzony z polaczeniem
        /// </summary>
        public override IPEndPoint EndPointAddress
        {
            get { return _endPointAddress; }
        }


        /// <summary>
        ///   Kontrukcja obiektu jesli polaczenie jest juz wczesniej ustanowione
        /// </summary>
        /// <param name = "client"></param>
        public RemotePeer(TcpClient client)
        {
            _client = client;
            _disconnectTimer = new Timer(DisconnectPeerEvent, null, maxIdleTime, Timeout.Infinite);

            initialize();
        }


        /// <summary>
        ///   Kontrukcja obiektu jesli polaczenie jeszcze nie jest ustanawiane
        /// </summary>
        public RemotePeer()

        {
            _client = new TcpClient();
            _disconnectTimer = new Timer(DisconnectPeerEvent, null, Timeout.Infinite, Timeout.Infinite);
            _messageQueue = new Queue<NetworkSend>();
            _execQueue = new Queue<AfterConnectExec>();
        }

        /// <summary>
        ///   Inicjalizacja majaca miejsce po nawiazaniu polaczenia
        /// </summary>
        private void initialize()
        {
            _stream = _client.GetStream();

            _inFormatter = new BinaryFormatter();
            _outFormatter = new BinaryFormatter();
            _endPointAddress = (IPEndPoint) _client.Client.RemoteEndPoint;


            _connected = true;
            _listeningThread = new Thread(ListenOnPeer);
            _listeningThread.Start();
        }

        /// <summary>
        ///   Tlumaczy obiekt na adres ip jesli jest to mozliwe
        /// </summary>
        /// <param name = "address"></param>
        /// <returns></returns>
        public static IPAddress ToIpAddress(object address)
        {
            if (address is IPAddress)
            {
                return (IPAddress) address;
            }
            if (address is UnifiedAddress)
            {
                return ExtractAddress((UnifiedAddress) address);
            }
            return null;
        }

        /// <summary>
        ///   Obiekt przekazywany do funkcji zwrotnej ustanawiania polaczenia
        /// </summary>
        private class CallbackStateObject
        {
            public object stateObject { get; set; }

            public ConnectionResult callback { get; set; }


            public CallbackStateObject(object stateObject, ConnectionResult callback)
            {
                this.stateObject = stateObject;
                this.callback = callback;
            }
        }

        /// <summary>
        ///   Rozpoczyna nieblokujaca probe polaczenia
        /// </summary>
        /// <param name = "address">Adres na ktory ma nastapic polaczenie</param>
        /// <param name = "stateObject">Obiekt stanu</param>
        /// <param name = "callback">Funkcja zwrotna</param>
        public void BeginConnectAsync(IPAddress address, object stateObject, ConnectionResult callback)
        {
            
                _client.BeginConnect(address, NetworkModule.portNr, EndConnectAsync,
                                     new CallbackStateObject(stateObject, callback));
            
        }

        /// <summary>
        ///   Konczy probe polaczenia
        /// </summary>
        /// <param name = "result">wynik</param>
        public void EndConnectAsync(IAsyncResult result)
        {
            //      RemotePeer peer;
            var callbackHelp = (CallbackStateObject) result.AsyncState;
            bool success;
            try
            {
                _client.EndConnect(result);
                initialize();
                //  peer = this;
                success = true;
            }
            catch (SocketException)
            {
                //   peer = null;
                success = false;
            }

            callbackHelp.callback(this, callbackHelp.stateObject, success);
        }


        /// <summary>
        ///   Zleca wykonanie kodu po udanym polaczeniu
        /// </summary>
        /// <param name = "exec">Funkcja do wykonania</param>
        public override void AfterConnect(AfterConnectExec exec)
        {
            if (Connected)
            {
                exec();
            }
            else
            {
                _execQueue.Enqueue(exec);
            }
        }


        /// <summary>
        ///   Funkcja wywolywana przez timer po okreslonym czasie
        /// </summary>
        /// <param name = "data"></param>
        private void DisconnectPeerEvent(object data)
        {
            if (!_persistent)
            {

                Disconnect();
            }
        }


        public void SendAwaitingMessages()
        {
            NetworkSend send;
            while (_messageQueue.Count > 0)
            {
                send = _messageQueue.Dequeue();
                SendMessage(send);
            }

            AfterConnectExec exec;
            while (_execQueue.Count > 0)
            {
                exec = _execQueue.Dequeue();
                exec();
            }
        }


        private void ListenOnPeer()
        {
            // var formatter = new BinaryFormatter();
            //  var inFormatter = new BinaryFormatter();
            try
            {
                while (_connected)
                {
                    object obj = _inFormatter.Deserialize(_stream);

                    TryRestartTimer();

                    OnObjectReceived(obj);
                }
            }
            catch (Exception)
            {
                if (_connected)
                {
                    _disconnectTimer.Dispose();
                    _stream.Close();
                    _client.Close();

                    if (ConnectionLost != null)
                    {
                        ConnectionLost(this);
                    }

                    //OnObjectReceived(new ObjectReceivedEventArgs(new ConnectionLost()));
                }
                _connected = false;
            }
        }

        public static IPAddress ExtractAddress(UnifiedAddress dnsOrIP)
        {
            try
            {
                return IPAddress.Parse(dnsOrIP.address);
            }
            catch (FormatException)
            {
                return Dns.GetHostEntry(dnsOrIP.address).AddressList[0];
            }
        }

        public override void Disconnect()
        {
            if (_client.Connected)
            {
                _stream.Close();
                _client.Close();
            }
        }

        private void SendMessage(NetworkSend netMessage)
        {
            TryRestartTimer();


            if (netMessage.message is TaskMessage)
            {
                var task = (TaskMessage) netMessage.message;

            }
            else
            {

            }


            try
            {
                _outFormatter.Serialize(_client.GetStream(), netMessage);
            }
            catch (Exception e)
            {
                //NetworkModule.SendNotification(e, NotificationType.ERROR);
                // P.pr(e);
            }
        }

        public override void Send(NetworkSend message)
        {
            if (_client.Connected)
            {
                SendMessage(message);
            }
            else
            {
                _messageQueue.Enqueue(message);
            }
        }

        // 
        public override void TryRestartTimer()
        {
            if (!Persistent)
            {
                try
                {
                    _disconnectTimer.Change(maxIdleTime, Timeout.Infinite);
                }
                catch (Exception)
                {
                    // timer jest Disposed, nic nie rob
                }
            }
        }
    }
}