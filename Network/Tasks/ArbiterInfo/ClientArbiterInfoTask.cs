// Author: Piotr Trzpil
namespace Network.Tasks.ArbiterInfo
{
    #region Usings

    using System.Connections;
    using System.MainParts;
    using Filechronization.Modularity.Messages;
    using global::System.Net;
    using Messages;

    #endregion

    #region Usings

    #endregion

    /// <summary>
    ///   Zadanie zapytania innego komputera o adres arbitra
    /// </summary>
    public class ClientArbiterInfoTask : NetworkTask
    {
        /// <summary>
        ///   Tworzy nowe zadanie zapytania innego komputera o adres arbitra
        /// </summary>
        /// <param name = "netModule">Modul sieciowy</param>
        public ClientArbiterInfoTask(NetworkModule netModule)
            : base(netModule, false)
        {
            AddHandler(typeof (ArbiterInfo), HandleArbiterInfo, 0);
        }

        public IPEndPoint Receiver
        {
            get
            {
                return key as IPEndPoint;
                //return (key is Peer) ? (Peer) key : null;
            }
        }
        public Peer PeerHandle
        {
            get
            {
                return key as Peer;
            }
        }
        /// <summary>
        ///   Rozpoczyna komunikacje wysylajac zapytanie o adres arbitra
        /// </summary>
        public void Start()
        {
            PeerHandle.Send( new ReqArbiterInfo());
        }

        /// <summary>
        ///   Obsluga otrzymanej wiadomosci z adresem arbitra
        /// </summary>
        /// <param name = "message">Wiadmosc z adresem arbitra</param>
        /// <returns>Nowa faza</returns>
        public int HandleArbiterInfo(Message message)
        {
            ArbiterInfo infoMessage = (ArbiterInfo) message;

            if (infoMessage.Arbiter == null)
            {
                _netModule.TaskCenter.BeginLogin(Receiver.Address);
            }
            else
            {
                _netModule.TaskCenter.BeginLogin(infoMessage.Arbiter.Address);
            }


            return PHASE_END;
        }

        public override bool CheckCondition()
        {
            return true;
        }
    }
}