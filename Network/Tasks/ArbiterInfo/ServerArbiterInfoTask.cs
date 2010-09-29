// Author: Piotr Trzpil
namespace Network.Tasks.ArbiterInfo
{
    #region Usings

    using System.Connections;
    using System.MainParts;
    using Filechronization.Modularity.Messages;
    using global::System.Net;
    using Messages;
    using States;

    #endregion

    #region Usings

    #endregion

    internal class ServerArbiterInfoTask : NetworkTask
    {
        public ServerArbiterInfoTask(NetworkModule netModule)
            : base(netModule, false)
        {
            AddHandler(typeof (ReqArbiterInfo), HandleReqArbiterInfo, 0);
        }

        
        public Peer PeerHandle
        {
            get
            {
                return key as Peer;
            }
        }
        public override bool CheckCondition()
        {
            return _netModule.NetworkState is StateConnected;
        }

        public int HandleReqArbiterInfo(Message message)
        {
            ReqArbiterInfo taskMessage = (ReqArbiterInfo) message;
            StateConnected state = (StateConnected) _netModule.NetworkState;

            PeerHandle.Send( new ArbiterInfo(state.Arbiter));
            return PHASE_END;
        }
    }
}