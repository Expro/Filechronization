// Author: Piotr Trzpil

namespace FileModule.Tasks
{
    #region Usings

    using Filechronization.Tasks;

    #endregion

    #region Usings

    #endregion

    public abstract class FileModuleTask : Task
    {
        protected NewFileModule fileModule;

        protected FileModuleTask(NewFileModule fileModule, bool isUnique)
            : base(isUnique)
        {
            this.fileModule = fileModule;
        }

//        protected void SendMessage(IPEndPoint endpoint, Message message)
//        {
//            _netModule.PeerCenter.HandleNetworkSend(new NetworkSend(endpoint, CreateTaskMessage(message)));
//        }
    }
}