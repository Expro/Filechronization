/*
 * Author: Piotr Trzpil
 */
namespace Filechronization.Network.States
{
    #region Usings

    using System.MainParts;
    using global::System.Net;

    #endregion
    /// <summary>
    /// Reprezentuje stan polaczony sie sieci
    /// </summary>
    public abstract class StateConnected : StateAbstract
    {
        public readonly IPEndPoint Arbiter;

        protected StateConnected(NetworkModule netModule, IPEndPoint arbiter)
            : base(netModule)
        {
            Arbiter = arbiter;
        }

        public override void BuildTaskAssociations()
        {
//            messageAssociations.Add(typeof (UserStateChanged), HandleUserStateChanged);
//            messageAssociations.Add(typeof (FileModuleMessage), HandleFileModuleMessage);
        }

//
//
//        private void HandleUserStateChanged(User sender, Message message)
//        {
//            _netModule.SendToService(message);
//        }
//
//        private void HandleFileModuleMessage(User sender, Message message)
//        {
//            _netModule.SendToService(message);
//        }

//
//        private void handleNewFile(User sender, Message message)
//        {
//            var newFile = (NewFile) message;
//
        //            if(_netModule.peerCenter.connectTo(newFile.owner))
        //            {
//            _netModule.peerCenter.sendTo(newFile.owner, null, new FileBlockRequest());
        //            }
//        }
//
//        private void handleFileBlockRequest(User sender,  Message message)
//        {
//            var newFile = (FileBlockRequest) message;
//        }
    }
}