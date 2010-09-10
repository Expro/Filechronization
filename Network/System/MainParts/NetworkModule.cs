/*
 * Author: Piotr Trzpil
 */
 
#region Usings
using Filechronization.Tasks.Messages;
using Filechronization.Network.Messages;
using Filechronization.Modularity;
using Filechronization.Modularity.Messages;
using Filechronization.Network.States;
using Filechronization.UserManagement;
#endregion
 
namespace Filechronization.Network.System.MainParts
{
	/// <summary>
	///   Glowny obiekt modulu sieciowego
	/// </summary>
	public class NetworkModule
	{
		///<summary>
		///  Struktura przechowujaca uzytwkownikow.
		///</summary>
		public Users UsersStructure
		{
			get { return SharedContext.users; }
		}

		/// <summary>
		///   Modul sterujacy wiadomosciami
		/// </summary>
		public Service ServiceModule
		{
			get { return SharedContext.service; }
		}

		public static int portNr = 6112;

		public readonly NetQueue netQueue;


		private StateAbstract _networkState;

		public StateAbstract NetworkState
		{
			get { return _networkState; }
		}


		public PeerCenter PeerCenter { get; private set; }

		public TaskCenter TaskCenter { get; private set; }


		private InterfaceModuleLink _interfaceLink;
		private FileModulelLink _fileLink;

		/// <summary>
		///   Aktualny uzytkownik
		/// </summary>
		public User CurrentUser { get; set; }


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


		private void AssignProcessor(Service serv, Processor processor)
		{
			serv.AssignProcessor(typeof (NetworkSend), processor);
			serv.AssignProcessor(typeof (LocalTaskMessage), processor);
			serv.AssignProcessor(typeof (Notification), processor);


			serv.AssignProcessor(typeof (UserStateChanged), processor);
			serv.Register(typeof (UserStateChanged), HandleToStateMessage);
			serv.AssignProcessor(typeof (ConnectionLost), processor);
			serv.Register(typeof (ConnectionLost), HandleToStateMessage);


			//serv.Register(typeof (Notification), HandleNotification);
		}

		private void HandleToStateMessage(Message message)
		{
			var toState = (ToStateMessage) message;
			NetworkState.handleMessage((User) toState.UserSender, toState);
		}

		/// <summary>
		///   Zmiana stanu polaczenia z siecia na zadany
		/// </summary>
		/// <param name = "state">Zadany stan</param>
		public void ChangeStateTo(StateAbstract state)
		{
			SendNotification("State changed to:" + state.GetType().Name, NotificationType.STATE);
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