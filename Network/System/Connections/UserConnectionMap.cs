// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.System.Connections
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using Filechronization.UserManagement;
    using global::System.Collections.Generic;
    using MainParts;

    #endregion

    /// <summary>
    ///   Obiekt zawierajacy wzajemne mapowania uzytkownikow z ich polaczeniami
    /// </summary>
    public class UserConnectionMap
    {
        private readonly Dictionary<Peer, User> _connectedUsers;
        private readonly NetworkModule _netModule;
        private readonly Dictionary<User, Peer> _userConnections;

        private Peer _arbiterConnection;
        private User _arbiterUser;

        private User _currentUser;
        private Peer _currentUserConnection;

        public UserConnectionMap(NetworkModule netModule)
        {
            _netModule = netModule;

            _connectedUsers = new Dictionary<Peer, User>();
            _userConnections = new Dictionary<User, Peer>();
        }

        /// <summary>
        ///   Kojarzy ze soba uzytkownika i polaczenie
        /// </summary>
        /// <param name = "user">Dany uzytkownik</param>
        /// <param name = "peer">Dane polaczenie</param>
        public void LinkUserAndConnection(User user, Peer peer)
        {
            if (_connectedUsers.ContainsKey(peer))
            {
                return;
            }
            if (_userConnections.ContainsKey(user))
            {
                return;
            }
            _connectedUsers.Add(peer, user);
            _userConnections.Add(user, peer);
        }

        /// <summary>
        ///   Zapamietanie aktualnego uzytkownika bedacego arbitrem i jego polaczenie
        /// </summary>
        /// <param name = "arbiter">Dany uzytkownik</param>
        /// <param name = "peer">Dane polaczenie</param>
        public void LinkArbiter(User arbiter, Peer peer)
        {
            _arbiterUser = arbiter;
            _arbiterConnection = peer;
            _netModule.UsersStructure.arbiter = arbiter;
        }

        /// <summary>
        ///   Kojarzy lokalnego uzytkownika
        /// </summary>
        /// <param name = "local">Lokalny uzytkownik</param>
        /// <param name = "peer">Lokalne polaczenie</param>
        public void LinkCurrentUser(User local, LocalPeer peer)
        {
            _currentUser = local;
            _currentUserConnection = peer;
        }

        /// <summary>
        /// </summary>
        public void UnlinkArbiter()
        {
            _arbiterUser = null;
            _arbiterConnection = null;
            _netModule.UsersStructure.arbiter = null;
        }

        /// <summary>
        ///   Pobiera z mapy polaczenie zadanego uzytkownika
        /// </summary>
        /// <param name = "user">Dany uzytkownik</param>
        /// <returns>Polaczenie uzytkownika, lub null, jesli nie znaleziono</returns>
        public Peer GetConnection(User user)
        {
            if (_currentUser.Equals(user))
            {
                return _currentUserConnection;
            }
            Peer peer;
            if (!_userConnections.TryGetValue(user, out peer))
            {
                return null;
            }
            return peer;
        }

        /// <summary>
        ///   Pobiera z mapy uzytkownika skojarzonego z danym polaczeniem
        /// </summary>
        /// <param name = "peer">Dane polaczenie</param>
        /// <returns>uzytkownik skojarzony z danym polaczeniem, lub null, jesli nie znaleziono</returns>
        public User GetUser(Peer peer)
        {
            if (_currentUserConnection == peer)
            {
                return _currentUser;
            }

            User user;
            if (!_connectedUsers.TryGetValue(peer, out user))
            {
                // NetworkModule.Notification("Connection not linked to user: " + peer.Address, NotificationType.ERROR);
                return null;
            }
            return user;
        }


        /// <summary>
        ///   Jesli istnieje powiazanie usera z polaczeniem, to jest usuwane i metoda zwraca tego usera
        ///   przeciwnym przypadku zwraca null.
        /// </summary>
        /// <param name = "peer">Dane polaczenie</param>
        /// <returns>Uzytkownik lub null jesli nie bylo skojarzenia</returns>
        public User RemoveUserLink(RemotePeer peer)
        {
            User user;
            if (_connectedUsers.TryGetValue(peer, out user))
            {
                if (peer == _arbiterConnection)
                {
                    UnlinkArbiter();
                }


                _connectedUsers.Remove(peer);
                _userConnections.Remove(user);
                return user;
            }
            return null;
        }

        public bool ContainsUser(User user)
        {
            return _userConnections.ContainsKey(user);
        }

        /// <summary>
        ///   Wyslanie wiadomosci do wszystkich
        /// </summary>
        /// <param name = "message"></param>
        public void SendToAll(Message message)
        {
            foreach (Peer peer in _userConnections.Values)
            {
                peer.Send(message);
            }
        }

        /// <summary>
        ///   Wyslanie wiadomosci do arbitra
        /// </summary>
        /// <param name = "message"></param>
        public void SendToArbiter(Message message)
        {
            _arbiterConnection.Send(message);
        }
    }
}