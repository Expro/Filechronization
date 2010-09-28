/*
 * Author: Piotr Trzpil
 */
 using Filechronization.Tasks;
 
namespace Filechronization.Network.Tasks
{
    #region Usings

    using System.MainParts;
    using global::System.Net;
    using Messages;
    using Modularity.Messages;

    #endregion

    public abstract class NetworkTask : Task
    {
        protected readonly NetworkModule _netModule;

        protected NetworkTask(NetworkModule netModule, bool isUnique) : base(isUnique)
        {
            _netModule = netModule;
        }

//        protected void SendMessage(IPEndPoint endpoint, Message message)
//        {
//            _netModule.PeerCenter.HandleNetworkSend(new NetworkSend(endpoint, CreateTaskMessage(message)));
//        }
    }
}