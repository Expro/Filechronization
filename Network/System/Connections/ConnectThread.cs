// Author: Piotr Trzpil
namespace Network.System.Connections
{
    #region Usings

    using Filechronization.UserManagement;
    using global::System.Net.Sockets;

    #endregion

    #region Usings

    #endregion

    public delegate void ConnectionAcceptedEventHandler(TcpClient client);

    public delegate void ConnectionResult(RemotePeer peer, object stateObject, bool success);

    /// <summary>
    ///   Obiekt obslugujacy zawieranie polaczen
    /// </summary>
    public class ConnectThread
    {
        //public event ConnectionAcceptedEventHandler ConnectionAccepted;

        #region Delegates

        public delegate void UserChosenCallback(object user, RemotePeer peer);

        #endregion

        public void tryConnect(Users userList, User currentUser, UserChosenCallback callback)
        {
            new ConnectionGroup(userList, currentUser, callback).Run();
        }
    }
}