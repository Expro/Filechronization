/*
 * Author: Piotr Trzpil
 */
namespace Filechronization.Network.Tasks.ArbiterInfo
{
    #region Usings

    using System.MainParts;
    using global::System.Net;
    using Messages;
    using Modularity.Messages;
    using States;

    #endregion

    internal class ServerArbiterInfoTask : NetworkTask
    {
        public ServerArbiterInfoTask(NetworkModule netModule)
            : base(netModule, false)
        {
            AddHandler(typeof (ReqArbiterInfo), HandleReqArbiterInfo, 0);
        }

        public override bool CheckCondition()
        {
            return _netModule.NetworkState is StateConnected;
        }

        public int HandleReqArbiterInfo(Message message)
        {
            var taskMessage = (ReqArbiterInfo) message;
            var state = (StateConnected) _netModule.NetworkState;

            SendMessage(Receiver, new ArbiterInfo(state.Arbiter));
            return PHASE_END;
        }


        public IPEndPoint Receiver
        {
            get { return key as IPEndPoint; }
        }
    }
}