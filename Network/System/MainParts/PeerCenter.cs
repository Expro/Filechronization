/*
 * Author: Piotr Trzpil
 */

#region Usings
using Filechronization.Network.System.Connections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Filechronization.Network.Messages;
using Filechronization.Modularity.Messages;
using Filechronization.Network.States;
using Filechronization.UserManagement;
#endregion

namespace Filechronization.Network.System.MainParts
{
  	/// <summary>
    ///   Modul zarzadzajacy polaczeniami i ich skojarzeniami z uzytkownikami
    /// </summary>
    public class PeerCenter
    {
        private readonly NetworkModule _netModule;

        /// <summary>
        ///   wszystkie zfinalizowane polaczenia
        /// </summary>
        private readonly Dictionary<IPEndPoint, RemotePeer> _connections;

        /// <summary>
        ///   polaczenia jeszcze nie ustanowione
        /// </summary>
        private readonly Dictionary<IPAddress, RemotePeer> _unfinishedConnections;

        /// <summary>
        ///   mapa asocjacji polaczen z uzytkownikami
        /// </summary>
        private readonly UserConnectionMap _userConnectionMap;


        /// <summary>
        ///   Kolejka obslugiwana przez glowny watek programu
        /// </summary>
        private readonly NetQueue _netQueue;

        /// <summary>
        ///   Obiekt zajmujacy sie polaczeniami na nizszym poziomie
        /// </summary>
        private readonly ConnectThread _connectThread;


        private readonly Timer _networkConnectTimer;

        /// <summary>
        ///   Obiekt polaczenia zwrotnego.
        /// </summary>
        private readonly LocalPeer _loopback;

        /// <summary>
        ///   Tworzy modul zarzadzajacy polaczeniami
        /// </summary>
        /// <param name = "netModule"></param>
        public PeerCenter(NetworkModule netModule)
        {
            _netModule = netModule;
            _netQueue = netModule.netQueue;
            _connections = new Dictionary<IPEndPoint, RemotePeer>();
            _unfinishedConnections = new Dictionary<IPAddress, RemotePeer>();


            _loopback = new LocalPeer(_netModule);
            _loopback.ObjectReceived += ObjectReceived;
            _userConnectionMap = new UserConnectionMap(_netModule);


            _connectThread = new ConnectThread();

            _connectThread.ConnectionAccepted += ConnectionAccepted;


            // _netQueue.Register(typeof (NetworkSend), HandleNetworkSend);


            _networkConnectTimer = new Timer(NextConnect, null, Timeout.Infinite, Timeout.Infinite);
        }


        /// <summary>
        ///   rozpoczecie prob laczenia sie do sieci
        /// </summary>
        public void StartConnectToNetwork()
        {
            if (!UserAddressesControl.SarbiterCheckBox.Checked)
            {
                _networkConnectTimer.Change(500, Timeout.Infinite);
            }
            else
            {
                ChangeToArbiter();
            }
        }

        /// <summary>
        ///   kolejna proba polaczenia do sieci
        /// </summary>
        /// <param name = "state">== null</param>
        private void NextConnect(object state)
        {
            _netQueue.Add(
                delegate
                    {
                        if (_netModule.NetworkState is StateDisconnected)
                        {
                            //NetworkModule.SendNotification("Trying connect to network...",
                                                           NotificationType.DIAGNOSTIC);
                            _connectThread.tryConnect(_netModule.UsersStructure, _netModule.CurrentUser,
                                                      UserChosen);
                        }
                        else
                        {
                            _networkConnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        }
                    });
        }


        /// <summary>
        ///   Przejscie do stanu arbitra i wlaczenie nasluchiwania polaczen
        /// </summary>
        public void ChangeToArbiter()
        {
            _netModule.ChangeStateTo(new StateArbiter(_netModule));
            _netModule.CurrentUser.currentAddress = new IPEndPoint(IPAddress.Loopback, NetworkModule.portNr);
            _userConnectionMap.LinkArbiter(_netModule.CurrentUser, _loopback);
            _userConnectionMap.LinkCurrentUser(_netModule.CurrentUser, _loopback);
            //NetworkModule.SendNotification("Starting server", NotificationType.DIAGNOSTIC);
            Console.WriteLine("DD");
            _connectThread.StartServer();
        }

        /// <summary>
        ///   zlecenie utworzenia polaczenia na dany adres
        /// </summary>
        /// <param name = "address">dany adres</param>
        /// <returns></returns>
        public Peer BeginConnect(IPAddress address)
        {
            RemotePeer peer=null;
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


        //#######################################
        // Metody obslugi zawierania polaczen: (wywolywane sa z obcych watkow)
        //#######################################


        /// <summary>
        ///   Wywolywana w przypadku udanej lub nie proby laczenia sie do uzytkownika w sieci.
        /// </summary>
        /// <param name = "userInfo">login uzytkownika z ktorym udalo sie polaczyc, lub null w przeciwnym przypadku</param>
        /// <param name = "peer">Obiekt udanego polaczenia, lub null w przeciwnym przypadku</param>
        public void UserChosen(object userInfo, RemotePeer peer)
        {
            _netQueue.Add(
                delegate
                    {
                        if (peer != null)
                        {
                            User user = _netModule.UsersStructure[(string) userInfo];

                            //  P.pr("Connected to user: " + user.login + " - " + peer.Address, Color.Blue);
                            //NetworkModule.SendNotification(
                                "Connected to user: " + user.login + " - " + peer.EndPointAddress,
                                NotificationType.RESOURCE);

                            peer = AddSuccessfulConnection(peer);


                            _netModule.TaskCenter.StartConnectionTask(peer);
                        }
                        else
                        {
                            SharedContext.service.EnqueueMessage(new ToInterfaceLoginResult(true, null));
                            ChangeToArbiter();
                        }
                    });
        }


        /// <summary>
        ///   Wywolywana przy otrzymaniu polaczenia
        /// </summary>
        /// <param name = "client">Obiekt clienta z ktorego nadeszlo polaczenie</param>
        public void ConnectionAccepted(TcpClient client)
        {
            _netQueue.Add(
                delegate
                    {
                        // P.pr("Accepted from: " + client.Client.RemoteEndPoint, Color.Blue);
                        //NetworkModule.SendNotification("Accepted from: " + client.Client.RemoteEndPoint,
                                                       NotificationType.RESOURCE);

                        var peer = new RemotePeer(client);
                        AddSuccessfulConnection(peer);

                        foreach (User user in _netModule.UsersStructure)
                        {
                            if (user.isConnected && peer.EndPointAddress.Address.Equals(user.currentAddress.Address))
                            {
                                _userConnectionMap.LinkUserAndConnection(user, peer);
                                break;
                            }
                        }
                    });
        }


        /// <summary>
        ///   wywolywana przy udanym lub nie polaczeniu z peerem zadanym w metodzie BeginConnect
        /// </summary>
        /// <param name = "peer">Obiekt zleconego polaczenia</param>
        /// <param name = "stateObject">adres IP uzyty do polaczenia</param>
        /// <param name = "success">jesli polaczenie sie udalo, == true</param>
        public void PeerConnectionResult(RemotePeer peer, object stateObject, bool success)
        {
            _netQueue.Add(
                delegate
                    {
                        if (success)
                        {
                            //P.pr("Nonblocking connection established.", Color.Blue);
                            //NetworkModule.SendNotification("Successfully connected to peer: " + peer.EndPointAddress,
                                                           NotificationType.RESOURCE);
                            peer = AddSuccessfulConnection(peer);
                            peer.SendAwaitingMessages();
                        }
                        else
                        {
                            //NetworkModule.SendNotification("Cannot connect to peer: " + stateObject,
                                                           NotificationType.RESOURCE);
                        }

                        _unfinishedConnections.Remove((IPAddress) stateObject);
                    });
        }


        /// <summary>
        ///   Nastapilo rozlaczenie ustanowionego wczesniej polaczenia
        /// </summary>
        /// <param name = "peer">Obiekt polaczenia, ktore zostalo przerwane</param>
        public void ConnectionLost(RemotePeer peer)
        {
            //NetworkModule.SendNotification("Lost connection with: " + peer.EndPointAddress, NotificationType.RESOURCE);

            _netQueue.Add(
                delegate
                    {
                        if (_netModule.NetworkState is StateNonArbiter)
                        {
                            var state = (StateNonArbiter) _netModule.NetworkState;
                            RemotePeer p;
                            if (_connections.TryGetValue(state.Arbiter, out p) && p == peer)
                                //if(_connections[state.Arbiter] == peer)
                            {
                                _netQueue.Add(() => _netModule.ChangeStateTo(new StateDisconnected(_netModule)));
                            }
                        }


                        User user = RemoveConnection(peer);

                        // jesli z tym polaczeniem byl skojazony user, obiekt przekazywany jest glebiej
                        if (user != null)
                        {
                            _netModule.TaskCenter.ObjectReceived(peer, user, new ConnectionLost());
                        }
                    });
        }


        //#######################################


        /// <summary>
        ///   Dodaje polaczenie do listy i rejestruje procedury obslugi zdarzen.
        ///   wywolywana gdy w module potrzebna jest wiedza o tym polaczeniu czyli:
        ///   po wybraniu peera do ktorego nastapi poczatek laczenia sie do sieci
        ///   lub przy odebraniu polaczenia
        ///   lub przy tworzeniu nieustanowionego jeszcze polaczenia DO usera w przypadku proby wyslania do niego wiadomosci
        /// </summary>
        /// <param name = "remotePeer">Obiekt polaczenia</param>
        /// <returns></returns>
        private RemotePeer AddSuccessfulConnection(RemotePeer remotePeer)
        {
            remotePeer.ObjectReceived += ObjectReceived;
            remotePeer.ConnectionLost += ConnectionLost;
            remotePeer.TryRestartTimer();
            try
            {
                _connections.Add(remotePeer.EndPointAddress, remotePeer);
            }
            catch (ArgumentException)
            {
                RemotePeer previousPeer = _connections[remotePeer.EndPointAddress];
                remotePeer.Disconnect();
                return previousPeer;
            }
            return remotePeer;
        }

        /// <summary>
        ///   Usuniecie polaczenia z listy, jesli polaczenie bylo przypisane do usera, jest on zwracany
        /// </summary>
        /// <param name = "peer">obiekt polaczenia do usuniecia</param>
        /// <returns>obiekt usera (jesli polaczenie bylo przypisane do usera) lub null</returns>
        private User RemoveConnection(RemotePeer peer)
        {
            _connections.Remove(peer.EndPointAddress);
            //NetworkModule.SendNotification("Connections left: " + _connections.Count, NotificationType.DIAGNOSTIC);

            return _userConnectionMap.RemoveUserLink(peer);
        }

        /// <summary>
        ///   Metoda wywolywana w przypadku przyjscia obiektu z jednego z polaczen
        /// </summary>
        /// <param name = "peer">Polaczenie, ktore otrzymalo obiekt z sieci</param>
        /// <param name = "netMessage">Otrzymana wiadomosc</param>
        public void ObjectReceived(Peer peer, NetworkSend netMessage)
        {
            _netQueue.Add(
                delegate
                    {
                        User user = _userConnectionMap.GetUser(peer);
                        _netModule.TaskCenter.ObjectReceived(peer, user, netMessage.message);
                    });
        }


        /// <summary>
        ///   wyslanie wiadomosci do usera podlaczonego do sieci, polaczenie z userem nie musi byc wczesniej ustanowione
        /// </summary>
        /// <param name = "user">uzytkownik, do ktorego ma zostac wyslane polaczenie</param>
        /// <param name = "message">wiadomosc do wyslania</param>
        public void SendTo(User user, Message message)
        {
            Peer peer = _userConnectionMap.GetConnection(user);

            if (peer == null)
            {
                if (user.isConnected)
                {
                    peer = BeginConnect(user.currentAddress.Address);
                    _userConnectionMap.LinkUserAndConnection(user, peer);
                }
                else
                {
                    //NetworkModule.SendNotification("User address not known: " + user.login, NotificationType.RESOURCE);
                    return;
                }
            }

            peer.Send(new NetworkSend(peer.EndPointAddress, message));
        }


        public void HandleNetworkSend(NetworkSend netSend)
        {
            RemotePeer peer;
            if (_connections.TryGetValue(netSend.reciver, out peer))
            {
                peer.Send(netSend);
            }
        }


        /// <summary>
        ///   Metoda wykonywana przez arbitra w przypadku pomyslnej autoryzacji uzytkownika
        /// </summary>
        /// <param name = "user">Poprawnie zautoryzowany uzytkownik</param>
        /// <param name = "address">Adres zwiazany z polaczeniem uzytkownika</param>
        public void EndUserLogin(User user, IPEndPoint address)
        {
            RemotePeer peer = _connections[address];
            peer.Persistent = true;
            //        user.currentAddress = address;
            _userConnectionMap.SendToAll(new UserStateChanged(user.login, UserState.ONLINE, address));

            _userConnectionMap.LinkUserAndConnection(user, peer);
        }

        /// <summary>
        ///   Wyslanie wiadomosci do wszystkich z wyjatkiem lokalnego uzytkownika
        /// </summary>
        /// <param name = "message">Wiadomosc do wyslania</param>
        public void SendToAll(Message message)
        {
            if (_netModule.NetworkState is StateConnected)
            {
                foreach (User user in _netModule.UsersStructure.connectedUsers)
                {
                    if (!user.Equals(_netModule.CurrentUser))
                    {
                        SendTo(user, message);
                    }
                }

                //_userConnectionMap.SendToAll(message);
            }
        }

        /// <summary>
        ///   Wyslanie wiadomosci do wszystkich z wyjatkiem podanego uzytkownika
        /// </summary>
        /// <param name = "message">Wiadomosc do wyslania</param>
        /// <param name = "excludedUser">Uzytkownik wylaczony z wysylania</param>
        public void SendToAllBut(Message message, User excludedUser)
        {
            if (_netModule.NetworkState is StateConnected)
            {
                foreach (User user in _netModule.UsersStructure.connectedUsers)
                {
                    if (!user.Equals(excludedUser))
                    {
                        SendTo(user, message);
                    }
                }
            }
        }

        /// <summary>
        ///   Wykonywana przez uzytkownika jesli udalo mu sie poprawnie zalogowac do sieci
        /// </summary>
        /// <param name = "arbiterAddress">Adres arbitra</param>
        /// <param name = "arbiterLogin">Login arbitra</param>
        /// <param name = "userAddresses">Mapa z adresami aktualnie zalogowanych uzytkownikow</param>
        public void EndLoginToNetwork(IPEndPoint arbiterAddress, string arbiterLogin,
                                      Dictionary<string, IPAddress> userAddresses)
        {
            User arbiter = _netModule.UsersStructure[arbiterLogin];

            foreach (var pair in userAddresses)
            {
                IPEndPoint endpoint = pair.Value == null ? null : new IPEndPoint(pair.Value, NetworkModule.portNr);
                _netModule.UsersStructure[pair.Key].currentAddress = endpoint;
            }

            _netModule.ChangeStateTo(new StateNonArbiter(_netModule, arbiterAddress));


            Peer connection = _connections[arbiterAddress];
            connection.Persistent = true;
            arbiter.currentAddress = arbiterAddress;
            _userConnectionMap.LinkArbiter(arbiter, connection);
            _userConnectionMap.LinkUserAndConnection(arbiter, connection);
            _userConnectionMap.LinkCurrentUser(_netModule.CurrentUser, _loopback);

            _connectThread.StartServer();
        }


        /// <summary>
        ///   Wyslanie wiadomosci do arbitra
        /// </summary>
        /// <param name = "message">Wiadomosc do wyslania</param>
        public void SendToArbiter(Message message)
        {
            if (_netModule.NetworkState is StateConnected)
            {
                _userConnectionMap.SendToArbiter(message);
            }
        }
    }
}