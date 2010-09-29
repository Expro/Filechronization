// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.System.MainParts
{
    #region Usings

    using Filechronization.Modularity;
    using Filechronization.Modularity.Messages;
    using Filechronization.Tasks.Messages;
    using Filechronization.UserManagement;
    using Messages;
    using States;

    #endregion

    /// <summary>
    ///   Glowny obiekt modulu sieciowego
    /// </summary>
    public class NetworkModule
    {
        public static int portNr = 6112;

        public readonly NetQueue netQueue;
        private FileModulelLink _fileLink;
        private InterfaceModuleLink _interfaceLink;


        private StateAbstract _networkState;

        /// <summary>
        ///   Tworzy modul sieciowy
        /// </summary>
        public NetworkModule()
        {
            Processor proc = ServiceModule.CreateProcessor();

            AssignProcessor(ServiceModule, proc);

            netQueue = new NetQueue(ServiceModule, proc);
            netQueue.Start();

            _interfaceLink = new InterfaceModuleLink(this);
            _fileLink = new FileModulelLink(this);
            PeerCenter = new PeerCenter(this);
            TaskCenter = new TaskCenter(this);


            _networkState = new StateDisconnected(this);
        }

        ///<summary>
        ///  Struktura przechowujaca uzytwkownikow.
        ///</summary>
        public Users UsersStructure
        {
            get { return null; }
        }

        /// <summary>
        ///   Modul sterujacy wiadomosciami
        /// </summary>
        public Service ServiceModule
        {
            get { return null; }
        }

        public StateAbstract NetworkState
        {
            get { return _networkState; }
        }


        public PeerCenter PeerCenter { get; private set; }

        public TaskCenter TaskCenter { get; private set; }


        /// <summary>
        ///   Aktualny uzytkownik
        /// </summary>
        public User CurrentUser { get; set; }


        private void AssignProcessor(Service serv, Processor processor)
        {
            serv.AssignProcessor(typeof (NetworkSend), processor);
            serv.AssignProcessor(typeof (LocalTaskMessage), processor);
            //serv.AssignProcessor(typeof (Notification), processor);


            serv.AssignProcessor(typeof (UserStateChanged), processor);
            serv.Register(typeof (UserStateChanged), HandleToStateMessage);
            serv.AssignProcessor(typeof (ConnectionLost), processor);
            serv.Register(typeof (ConnectionLost), HandleToStateMessage);


            //serv.Register(typeof (Notification), HandleNotification);
        }

        private void HandleToStateMessage(Message message)
        {
            ToStateMessage toState = (ToStateMessage) message;
            NetworkState.handleMessage((User) toState.UserSender, toState);
        }

        /// <summary>
        ///   Zmiana stanu polaczenia z siecia na zadany
        /// </summary>
        /// <param name = "state">Zadany stan</param>
        public void ChangeStateTo(StateAbstract state)
        {
            //SendNotification("State changed to:" + state.GetType().Name, NotificationType.STATE);
            _networkState = state;
            if (NetworkState is StateDisconnected)
            {
                //PeerCenter.StartConnectToNetwork();
            }
        }


//
//		public void Dispose()
//		{
//			netQueue.Add(
//				delegate
//					{
//						if (NetworkState is StateArbiter)
//						{
//							((StateArbiter) NetworkState).giveAwayArbiter();
//						}
        // networkState.
//					});
//		}
    }
}