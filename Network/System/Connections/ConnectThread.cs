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
        

        //public event ConnectionAcceptedEventHandler ConnectionAccepted;

        public ConnectThread()
        {
            
            
        }

       

        public delegate void UserChosenCallback(object user, RemotePeer peer);


        


        public void tryConnect(Users userList, User currentUser, UserChosenCallback callback)
        {
            new ConnectionGroup(userList, currentUser, callback).Run();
        }
    }
}