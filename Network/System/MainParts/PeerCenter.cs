// Author: Piotr Trzpil

#region Usings



#endregion

/*
 * 
 * PeerCenter;
 * musi sie rejestrowac w NetworksManager
 * nie rozlacza polaczen
 * nie otrzymuje notyfikacji o pol. przychodzacych
 * - dopiero otrzymanie wiadomosci od inicjatora powoduje jego zauwazenie
 * laczy polaczenia z uzytkownikami
 * w przypadku rozlaczenia lub niemozliwosci polaczenia -
 * - otrzymuje notyfikacje ConnectionClosed
 */

namespace Network.System.MainParts
{
    #region Usings

    using Connections;
    using Filechronization.Modularity.Messages;
    using Filechronization.UserManagement;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Threading;
    using Messages;
    using Network.Connections;
    using States;

    #endregion

    /// <summary>
    ///   Modul zarzadzajacy polaczeniami i ich skojarzeniami z uzytkownikami
    /// </summary>
    public class PeerCenter
    {
        /// <summary>
        ///   Obiekt zajmujacy sie polaczeniami na nizszym poziomie
        /// </summary>
        private readonly ConnectThread _connectThread;

        /// <summary>
        ///   Obiekt polaczenia zwrotnego.
        /// </summary>
        private readonly LocalPeer _loopback;

        private readonly NetworkModule _netModule;


        /// <summary>
        ///   Kolejka obslugiwana przez glowny watek programu
        /// </summary>
        private readonly NetQueue _netQueue;


        private readonly Timer _networkConnectTimer;

        /// <summary>
        ///   mapa asocjacji polaczen z uzytkownikami
        /// </summary>
        private readonly UserConnectionMap _userConnectionMap;

        private NetworksManager _manager;

        /// <summary>
        ///   Tworzy modul zarzadzajacy polaczeniami
        /// </summary>
        /// <param name = "netModule"></param>
        public PeerCenter(NetworkModule netModule)
        {
            _netModule = netModule;
            _netQueue = netModule.netQueue;


            _loopback = new LocalPeer(_netModule);
            //_loopback.ObjectReceived += ObjectReceived;
            _userConnectionMap = new UserConnectionMap(_netModule);


            _connectThread = new ConnectThread();


            // _netQueue.Register(typeof (NetworkSend), HandleNetworkSend);


            _networkConnectTimer = new Timer(NextConnect, null, Timeout.Infinite, Timeout.Infinite);
        }


        /// <summary>
        ///   rozpoczecie prob laczenia sie do sieci
        /// </summary>
        public void StartConnectToNetwork()
        {
            throw new NotImplementedException();
//            if (!UserAddressesControl.SarbiterCheckBox.Checked)
//            {
//                _networkConnectTimer.Change(500, Timeout.Infinite);
//            }
//            else
//            {
//                ChangeToArbiter();
//            }
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
                        //                              NotificationType.DIAGNOSTIC);
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
            //_connectThread.StartServer();
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
//                                "Connected to user: " + user.login + " - " + peer.EndPointAddress,
//                                NotificationType.RESOURCE);


                        _netModule.TaskCenter.StartConnectionTask(peer);
                    }
                    else
                    {
                        //SharedContext.service.EnqueueMessage(new ToInterfaceLoginResult(true, null));
                        ChangeToArbiter();
                    }
                });
        }


        public void ConnectionAccepted(RemotePeer peer)
        {
            _netQueue.Add(
                delegate
                {
                    // P.pr("Accepted from: " + client.Client.RemoteEndPoint, Color.Blue);
                    //NetworkModule.SendNotification("Accepted from: " + client.Client.RemoteEndPoint,
//                                                       NotificationType.RESOURCE);


                    // To jest bez sensu:

                    foreach (User user in _netModule.UsersStructure)
                    {
                        if (user.isConnected && peer.Endpoint.Address.Equals(user.currentAddress.Address))
                        {
                            _userConnectionMap.LinkUserAndConnection(user, peer);
                            break;
                        }
                    }
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
                        StateNonArbiter state = (StateNonArbiter) _netModule.NetworkState;
                        RemotePeer p;


                        //TODO: bez sensu:
//                            if (_connections.TryGetValue(state.Arbiter, out p) && p == peer)
//                            {
//                                _netQueue.Add(() => _netModule.ChangeStateTo(new StateDisconnected(_netModule)));
//                            }
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
        ///   Usuniecie polaczenia z listy, jesli polaczenie bylo przypisane do usera, jest on zwracany
        /// </summary>
        /// <param name = "peer">obiekt polaczenia do usuniecia</param>
        /// <returns>obiekt usera (jesli polaczenie bylo przypisane do usera) lub null</returns>
        private User RemoveConnection(RemotePeer peer)
        {
            //NetworkModule.SendNotification("Connections left: " + _connections.Count, NotificationType.DIAGNOSTIC);

            return _userConnectionMap.RemoveUserLink(peer);
        }

        /// <summary>
        ///   Metoda wywolywana w przypadku przyjscia obiektu z jednego z polaczen
        /// </summary>
        /// <param name = "peer">Polaczenie, ktore otrzymalo obiekt z sieci</param>
        /// <param name = "netMessage">Otrzymana wiadomosc</param>
        public void ObjectReceived(Peer peer, Message message)
        {
            _netQueue.Add(
                delegate
                {
                    User user = _userConnectionMap.GetUser(peer);
                    _netModule.TaskCenter.ObjectReceived(peer, user, message);
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
                    peer = _manager.Connect(this, user.currentAddress.Address);
                    _userConnectionMap.LinkUserAndConnection(user, peer);
                }
                else
                {
                    //NetworkModule.SendNotification("User address not known: " + user.login, NotificationType.RESOURCE);
                    return;
                }
            }

            peer.Send(message);
        }


//        public void HandleNetworkSend(NetworkSend netSend)
//        {
//            RemotePeer peer;
//            if (_connections.TryGetValue(netSend.reciver, out peer))
//            {
//                peer.Send(netSend.message);
//            }
//        }


        /// <summary>
        ///   Metoda wykonywana przez arbitra w przypadku pomyslnej autoryzacji uzytkownika
        /// </summary>
        /// <param name = "user">Poprawnie zautoryzowany uzytkownik</param>
        /// <param name = "userConn">Adres zwiazany z polaczeniem uzytkownika</param>
        public void EndUserLogin(User user, Peer userConn)
        {
            userConn.Persistent = true;
            //        user.currentAddress = address;
            _userConnectionMap.SendToAll(new UserStateChanged(user.login, UserState.ONLINE, userConn.Endpoint));

            _userConnectionMap.LinkUserAndConnection(user, userConn);
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
        /// <param name = "arbiterConn">Polaczenie arbitra</param>
        /// <param name = "arbiterLogin">Login arbitra</param>
        /// <param name = "userAddresses">Mapa z adresami aktualnie zalogowanych uzytkownikow</param>
        public void EndLoginToNetwork(Peer arbiterConn, string arbiterLogin,
                                      Dictionary<string, IPAddress> userAddresses)
        {
            User arbiter = _netModule.UsersStructure[arbiterLogin];

            foreach (KeyValuePair<string, IPAddress> pair in userAddresses)
            {
                IPEndPoint endpoint = pair.Value == null ? null : new IPEndPoint(pair.Value, NetworkModule.portNr);
                _netModule.UsersStructure[pair.Key].currentAddress = endpoint;
            }

            _netModule.ChangeStateTo(new StateNonArbiter(_netModule, arbiterConn.Endpoint));


            arbiterConn.Persistent = true;
            arbiter.currentAddress = arbiterConn.Endpoint;
            _userConnectionMap.LinkArbiter(arbiter, arbiterConn);
            _userConnectionMap.LinkUserAndConnection(arbiter, arbiterConn);
            _userConnectionMap.LinkCurrentUser(_netModule.CurrentUser, _loopback);

//            _connectThread.StartServer();
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