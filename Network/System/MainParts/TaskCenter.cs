/*
 * Author: Piotr Trzpil
 */
 
#region Usings
using Filechronization.Network.System.Connections;
using Filechronization.Network.System.Exceptions;
using Filechronization.Tasks;
using Filechronization.Tasks.Messages;
using System;
using System.Net;
using Filechronization.Modularity.Messages;
using Filechronization.Network.Tasks.ArbiterInfo;
using Filechronization.Network.Tasks.ArbiterInfo.Messages;
using Filechronization.UserManagement;
#endregion
 
namespace Filechronization.Network.System.MainParts
{
    /// <summary>
    ///   Modul zajmujacy sie polaczeniami na wyzszym poziomie abstrakcji
    /// </summary>
    public class TaskCenter
    {
        private readonly NetworkModule _netModule;


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
            AddTaskInit(typeof (SaltRequest), () => new AuthorizationServerTask(_netModule, _netModule.UsersStructure));
            AddTaskInit(typeof (ReqArbiterInfo), () => new ServerArbiterInfoTask(_netModule));
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
            SharedContext.taskManager.AddInitializableTask(messageType, deleg);
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
                var taskMessage = (TaskMessage) message;

                if (user == null)
                {
                    if (!(taskMessage.message is AuthorizationAccepted || taskMessage.message is AuthorizationRejected
                          || taskMessage.message is SaltRequest
                          || taskMessage.message is SaltResponse || taskMessage.message is UserAuthorization
                          || taskMessage.message is ReqArbiterInfo || taskMessage.message is ArbiterInfo))
                    {
                        throw new BugException();
                    }


                    _netModule.ServiceModule.EnqueueMessage(new LocalTaskMessage(sender.EndPointAddress, taskMessage));
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
                        var userMessage = (UserMessage) message;
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
            var task = new ClientArbiterInfoTask(_netModule);
            SharedContext.taskManager.AddTask(peer.EndPointAddress, task);
            task.Start();
        }

        /// <summary>
        ///   Rozpoczecie zadania logowania do sieci
        /// </summary>
        /// <param name = "arbiter">adres arbitra</param>
        public void BeginLogin(IPAddress arbiter)
        {
            Peer peer = _netModule.PeerCenter.BeginConnect(arbiter);
            peer.AfterConnect(
                delegate
                    {
                        var task = new AuthorizationClientTask(_netModule, _netModule.CurrentUser);
                        SharedContext.taskManager.AddTask(peer.EndPointAddress, task);
                        task.Start();
                    });
        }


       
    }
}