// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.System.MainParts
{
    #region Usings

    using Connections;
    using Exceptions;
    using Filechronization.Modularity.Messages;
    using Filechronization.Tasks;
    using Filechronization.Tasks.Messages;
    using Filechronization.UserManagement;
    using global::System;
    using global::System.Net;
    using Network.Connections;
    using Tasks.ArbiterInfo;
    using Tasks.ArbiterInfo.Messages;
    using Tasks.Authorization;
    using Tasks.Authorization.Messages;

    #endregion

    /// <summary>
    ///   Modul zajmujacy sie polaczeniami na wyzszym poziomie abstrakcji
    /// </summary>
    public class TaskCenter
    {
        private readonly NetworkModule _netModule;
        private ConnectionManagerHigher _managerHigher;

        public TaskCenter(NetworkModule netModule)
        {
            _netModule = netModule;


            AssignTaskInitiators();
        }

        /// <summary>
        ///   Rejestracja typow wiadomosci, mogacych stworzyc zadanie
        /// </summary>
        private void AssignTaskInitiators()
        {
            AddTaskInit(typeof (SaltRequest), () => new AuthorizationServerSymTask(_netModule, _netModule.UsersStructure));
            AddTaskInit(typeof (ReqArbiterInfo), () => new ServerArbiterInfoSymTask(_netModule));
        }

        /// <summary>
        ///   Skrot do rejestracji
        /// </summary>
        /// <param name = "messageType"></param>
        /// <param name = "deleg"></param>
        private void AddTaskInit(Type messageType, TaskCreator deleg)
        {
            if (!messageType.IsSubclassOf(typeof (Message)))
            {
                throw new ArgumentException();
            }
            Global.TaskManager.AddInitializableTask(messageType, deleg);
        }


        /// <summary>
        ///   Otrzymano wiadomosc od konkretnego polaczenia, byc moze skojarzonego z uzytkownikiem
        ///   Jesli jest to wiadomosc zwiazana z zadaniem, enkapsulowana jest w LocalTaskMessage, z kluczem uzytkownikiem, jesli
        ///   z polaczeniem byl taki zwiazany, lub z kluczem adresem, jesli nie.
        ///   Jesli nie jest to wiadomosc zwiazana z zadaniem, i nadeszla od konkretnego uzytkownika, wypelniane jest jej pole UserSender.
        ///   Nastepnie wiadomosc przekazywana jest do modulu komunikacyjnego
        /// </summary>
        /// <param name = "sender">Polaczenie z ktorego nadeszla wiadomosc</param>
        /// <param name = "user">Uzytkownik skojarzony z polaczeniem, lub null w przeciwnym przypadku</param>
        /// <param name = "message">Otrzymana wiadomosc</param>
        public void ObjectReceived(Peer sender, User user, Message message)
        {
            if (message is TaskMessage)
            {
                TaskMessage taskMessage = (TaskMessage) message;

                if (user == null)
                {
                    if (!(taskMessage.message is AuthorizationAccepted || taskMessage.message is AuthorizationRejected
                          || taskMessage.message is SaltRequest
                          || taskMessage.message is SaltResponse || taskMessage.message is UserAuthorization
                          || taskMessage.message is ReqArbiterInfo || taskMessage.message is ArbiterInfo))
                    {
                        throw new BugException();
                    }


                    _netModule.ServiceModule.EnqueueMessage(new LocalTaskMessage(sender, taskMessage));
                    ////NetworkModule.SendNotification("Received FileTaskMessage and user == null", NotificationType.ERROR);
                }
                else
                {
                    _netModule.ServiceModule.EnqueueMessage(new LocalTaskMessage(user, taskMessage));
                }
            }
            else
            {
                //     var user = _netModule.PeerCenter.GetUser(sender);
                if (user != null)
                {
                    if (message is UserMessage)
                    {
                        UserMessage userMessage = (UserMessage) message;
                        userMessage.UserSender = user;
                    }


                    _netModule.ServiceModule.EnqueueMessage(message);
                }
                else
                {
                    //NetworkModule.SendNotification("Ignored object: " + message.GetType().Name, NotificationType.WARNING);
                }
            }
        }

        /// <summary>
        ///   Rozpoczecie zadania zapytania o adres arbitra
        /// </summary>
        /// <param name = "peer">Polaczenie ktore ma zostac zapytane</param>
        public void StartConnectionTask(RemotePeer peer)
        {
            ClientArbiterInfoSymTask task = new ClientArbiterInfoSymTask(_netModule);
            Global.TaskManager.AddTask(peer, task);
            task.Start();
        }

        /// <summary>
        ///   Rozpoczecie zadania logowania do sieci
        /// </summary>
        /// <param name = "arbiter">adres arbitra</param>
        public void BeginLogin(IPAddress arbiter)
        {
            _managerHigher.ConnectTask(_netModule.NetworkManager, arbiter)
                .ContinueWith(prev =>
                {
                    AuthorizationClientSymTask task = new AuthorizationClientSymTask(_netModule, _netModule.CurrentUser);
                    Global.TaskManager.AddTask(prev.Result, task);
                    task.Start();
                });
        }
    }
}