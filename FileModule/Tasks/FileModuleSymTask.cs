// Author: Piotr Trzpil

namespace FileModule.Tasks
{
   


    public abstract class FileModuleSymTask : Filechronization.Tasks.SymTask
    {
        protected NewFileModule fileModule;

        protected FileModuleSymTask(NewFileModule fileModule, bool isUnique)
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