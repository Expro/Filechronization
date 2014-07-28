// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.System.Connections
{
    #region Usings

    using Filechronization.CommonClasses;
    using Filechronization.UserManagement;
    using global::System.Threading;

    #endregion

    /// <summary>
    ///   Obiekt tworzony przy probie laczenia sie do jednego z uzytkownikow w celu zalogowania sie do sieci
    /// </summary>
    public class ConnectionGroup
    {
        private readonly ConnectThread.UserChosenCallback _callback;
        private readonly User _currentUser;
        private readonly Users _usersToConnect;


        private int _callbacksCounter;
        private bool _connected;


        public ConnectionGroup(Users usersToConnect, User currentUser, ConnectThread.UserChosenCallback callback)
        {
            _usersToConnect = usersToConnect;
            _currentUser = currentUser;
            _callback = callback;
        }

        public void Run()
        {
//            foreach (User user in _usersToConnect)
//            {
//                if (!user.Equals(_currentUser))
//                    foreach (UnifiedAddress address in user.addresses)
//                    {
//                        new RemotePeer().BeginConnectAsync(RemotePeer.ToIpAddress(address), user.login,
//                                                           RemotePeerCreatedCallback);
//                    }
//            }
        }

        /// <summary>
        ///   Wywolywana przez Peera do ktorego powrocila funkcja laczaca
        /// </summary>
        /// <param name = "peer"></param>
        /// <param name = "stateObject"></param>
        /// <param name = "success"></param>
        private void RemotePeerCreatedCallback(RemotePeer peer, object stateObject, bool success)
        {
//            Monitor.Enter(this);
//
//            _callbacksCounter++;
//
//            if (success && !_connected)
//            {
//                _connected = true;
//                _callback(stateObject, peer);
//            }
//            else
//            {
//                peer.Disconnect();
//                if (!_connected && _callbacksCounter == _usersToConnect.Count - 1)
//                {
                    // z nikim sie nie polaczono
//                    _callback(null, null);
//                }
//            }
//
//
//            Monitor.Exit(this);
        }
    }
}