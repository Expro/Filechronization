/*
 * Author: Piotr Trzpil
 */
namespace Filechronization.Network.System.Connections
{
    #region Usings

    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Threading;
    using MainParts;
    using UserManagement;

    #endregion

    public delegate void ConnectionAcceptedEventHandler(TcpClient client);

    public delegate void ConnectionResult(RemotePeer peer, object stateObject, bool success);
    /// <summary>
    /// Obiekt obslugujacy zawieranie polaczen
    /// </summary>
    public class ConnectThread
    {
        private readonly Thread _connectThread;

        private readonly TcpListener _listener;


        public event ConnectionAcceptedEventHandler ConnectionAccepted;

        public ConnectThread()
        {
            _listener = new TcpListener(IPAddress.Any, NetworkModule.portNr);
            _connectThread = new Thread(ListenForConnections);
        }

        public void StartServer()
        {
            _connectThread.Start();
        }

        public delegate void UserChosenCallback(object user, RemotePeer peer);


        public void ListenForConnections()
        {
            _listener.Start();

            while (true)
            {
                try
                {
                    ConnectionAccepted(_listener.AcceptTcpClient());
                }
                catch (SocketException e)
                {
                    //NetworkModule.SendNotification(e, NotificationType.ERROR);
                }
            }
        }


        public void tryConnect(Users userList, User currentUser, UserChosenCallback callback)
        {
            new ConnectionGroup(userList, currentUser, callback).Run();
        }
    }
}