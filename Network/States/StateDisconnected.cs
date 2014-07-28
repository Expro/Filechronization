// Author: Piotr Trzpil
namespace Network.States
{
    #region Usings

    using System.MainParts;
    using Filechronization.UserManagement;

    #endregion

    #region Usings

    #endregion

    /// <summary>
    ///   Oznacza, ze nie ma polaczenia z siecia
    /// </summary>
    public class StateDisconnected : StateAbstract
    {
        public User arbiter;


        public StateDisconnected(NetworkModule netModule)
            : base(netModule)
        {
            //            connectTimer = new Timer(2000);
            //            connectTimer.Elapsed += TryConnect;
            //
            //            connectQueue = netModule.connectQueue;
            //            startConnect();
        }

        public override void BuildTaskAssociations()
        {
//            taskAssociations.Add(typeof (ReqArbiterInfo), typeof (ConnectionTask));
        }


        //        public void startConnect()
        //        {
        //            connected = false;
        //            netModule.connectList.ResetCounter();
        //            
        //            connectTimer.Start();
        //        }


        //        public void handleReqArbiterInfo(Message message)
        //        {
        //            var mess = (TaskMessage)message;
        //            var req = (ReqArbiterInfo)mess.message;
        //            var sender = mess.sender;
        //
        //            connectQueue.Add(
        //                delegate
        //                {
        //                    if (connected)
        //                    {
        //                        sender.Disconnect();
        //                    }
        //                    else
        //                    {
        //                        connected = true;
        //connectQueue.Clear();
        //                        connectTimer.Stop();
        //
        //                        _netQueue.Add(
        //                            delegate
        //                            {
        //netModule.ArbiterRemotePeer = new ArbiterModule(netModule);
        //                                sender.Send(new ArbiterInfo(netModule.currentUser, null));
        //                                connectQueue.Clear();
        //                                netModule.changeStateTo(new StateArbiter(netModule));
        //                            });
        //                    }
        //
        //                });
        //        }
        //        public void handleArbiterInfo(Message message)
        //        {
        //            var mess = (TaskMessage)message;
        //            var info = (ArbiterInfo)mess.message;
        //            var sender = mess.sender;
        //
        //
        //            if (info.Arbiter == null)
        //            {
        //                arbiter = sender;
        //            }
        //            else
        //            {
        //                sender.Disconnect();
        //                try
        //                {
        //                    arbiter = new RemotePeer(info.Arbiter);
        //                }
        //                catch (SocketException e)
        //                {
        //                    P.pr("Cant connect to arbiter, resetting connection task." + info.Arbiter);
        //                    startConnect();
        //                    return;
        //                }
        //                catch (Exception e)
        //                {
        //                    P.pr(e);
        //                    startConnect();
        //                    return;
        //                }
        //
        //            }
        //        }
        //        public void handleNetworkInfo(Message message)
        //        {
        //            var mess = (TaskMessage)message;
        //            var info = (NetworkInfo)mess.message;
        //            var sender = mess.sender;
        //
        //            P.pr("CONNECTED");
        //            connectQueue.Clear();
        //            netModule.changeStateTo(new StateNonArbiter(netModule, arbiter, info));
        //        }
        //        public void handleConnectionLost(Message message)
        //        {
        //            var mess = (TaskMessage)message;
        //            var info = (ConnectionLost)mess.message;
        //            var sender = mess.sender;
        //
        //            if (arbiter == null) // Stracono polaczenie przed uzyskaniem adresu arbitra.
        //            {
        //                P.pr("Stracono polaczenie przed uzyskaniem adresu arbitra.");
        //                startConnect();
        //            }
        //            else if (sender.Equals(arbiter)) // Stracono polaczenie z arbitrem.
        //            {
        //                P.pr("Stracono polaczenie z arbitrem.");
        //                startConnect();
        //            }
        // Stracono polaczenie z nie-arbitrem, po uzyskaniem adresu arbitra, nic nie rob.
        //        }
    }
}