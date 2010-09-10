/*
 * Author: Piotr Trzpil
 */
namespace Filechronization.Network.States
{
    #region Usings

    using System.MainParts;
    using global::System;
    using Messages;
    using Modularity.Messages;
    using UserManagement;

    #endregion
    /// <summary>
    /// Stan oznaczajacy, ze ten komputer jest arbitrem
    /// </summary>
    public class StateArbiter : StateConnected
    {
        public StateArbiter(NetworkModule netModule)
            : base(netModule, null)
        {
        }


        public override void BuildTaskAssociations()
        {
            base.BuildTaskAssociations();

       
            _messageAssociations.Add(typeof (ConnectionLost), HandleConnectionLost);


        }


        private void HandleConnectionLost(User sender, Message message)
        {
            sender.currentAddress = null;
            _netModule.PeerCenter.SendToAll(new UserStateChanged(sender.login, UserState.OFFLINE, null));
        }


//        private void handleFileAdditionAccepted(Message message)
//        {
//            _netModule.PeerCenter.SendToAll(new NewFile());
//        }

//        public void giveAwayArbiter()
//        {
//            User user = chooseNextArbiter();
            //_netModule.peerCenter.sendTo(user, null, new BecomeArbiterRequest());
//
//
            //_netModule.peerCenter.addTask(user, new ArbiterConnection(_netModule, _netModule.peerCenter.newId(), sender));
//
//            var taskId = _netModule.PeerCenter.newId();
//            var task = new GiveArbiterTask(_netModule, taskId, user);
//            if (_netModule.PeerCenter.addTask(user, task))
//            {
            //    _netModule.PeerCenter.sendTo(user, taskId, new BecomeArbiterRequest());
//            }
//        }
//
//        private User chooseNextArbiter()
//        {
//            throw new NotImplementedException();
//        }

        //        public void SearchForArbiters(object sender, ElapsedEventArgs elapsedEventArgs)
        //        {
        //            _netQueue.Add(
        //                delegate
        //                {
        //                    P.pr("Searching for split network...");
        //                    foreach (User user in netModule.users.disconnectedUsers)
        //                    {
        //                        if (!user.Equals(netModule.currentUser))
        //                        {
        //                            foreach (UnifiedAddress addr in user.staticAddresses)
        //                            {
        //                                try
        //                                {
        //                                    TryConnect(RemotePeer.ExtractAddress(addr));
        //                                }
        //                                catch (Exception e)
        //                                {
        //                                    
        //                                    P.pr(e);
        //                                }
        //                                
        //                            }
        //                        }
        //                    }
        //                });
        //        }
        //
        //        public void TryConnect(IPAddress addr)
        //        {
        //            connectQueue.Add(
        //                delegate
        //                {
        //                    try
        //                    {
        //                        var peer = new RemotePeer(addr);
        //
        //                        _netQueue.Add(
        //                            delegate
        //                            {
        //TODO: netModule.AddConnection(peer);
        //                                peer.Send(new ReqArbiterInfo(netModule.currentUser, 0));
        //                            });
        //                        
        //                    }
        //                    catch (SocketException )
        //                    {
        //                        P.pr("Cant connect to: " + addr);
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        P.pr(e);
        //                    }
        //                });
        //        }


        //        public void HandleReqNetworkInfo(Message message)
        //        {
        //            var mess = (TaskMessage)message;
        //            var info = (ReqNetworkInfo)mess.message;
        //            var sender = mess.sender;
        //
        //            sender.Send(new NetworkInfo(netModule.currentUser, netModule.users.connectedUsers));
        //
        //            netModule.users[info.user.login].currentAddress = sender.Address;
        //
        //            SendToAll(new NewPeer(netModule.currentUser, sender.Address));   
        //        }
        //
        //
        //        public void HandleReqArbiterInfo(Message message)
        //        {
        //            var mess = (TaskMessage)message;
        //            var info = (ReqArbiterInfo)mess.message;
        //            var sender = mess.sender;
        //
        //   
        //            sender.Send(new ArbiterInfo(netModule.currentUser, null));
        //        }
        //        public void HandleArbiterInfo(Message message)
        //        {
        //            var mess = (TaskMessage)message;
        //            var info = (ArbiterInfo)mess.message;
        //            var sender = mess.sender;
        //
        // 
        //            RemotePeer arbiter;
        //
        //            try
        //            {
        //                if (info.Arbiter == null)
        //                {
        //                    arbiter = sender;
        //                }
        //                else
        //                {
        //                    sender.Disconnect();
        //                    arbiter = new RemotePeer(info.Arbiter);
        //                }
        //                arbiter.Send(new ChangeArbiter(netModule.currentUser, null));
        //            }
        //            catch (Exception)
        //            {
        //                return;
        //            }
        //        }


        //        public void HandleChangeArbiter(Message message)
        //        {
        //            var mess = (TaskMessage)message;
        //            var info = (ChangeArbiter)mess.message;
        //            var sender = mess.sender;
        //
        //            SendToAll(new ChangeArbiter(netModule.currentUser, sender.Address));
        //        }
        //        public void HandleConnectionLost(Message message)
        //        {
        //            var mess = (TaskMessage)message;
        //            var info = (ConnectionLost)mess.message;
        //            var sender = mess.sender;
        //
        //            var user = netModule.users.findConnectedUser(sender.Address);
        //            if (user != null)
        //            {
        //
        //                SendToAll(new PeerLeft(netModule.currentUser, sender.Address));
        //                _connectedUsers.Remove(sender);
        //                user.currentAddress = null;
        //
        //                if (_connectedUsers.Count == 0)
        //                {
        //                    netModule.changeStateTo(new StateDisconnected(netModule));
        //                }
        //
        //            }
        //        }


        //        public void SendToAll(Message obj)
        //        {
        //            
        //            foreach (RemotePeer peer in _connectedUsers)
        //            {
        //                
        //            }
        //        }
        //        public void DisconnectAll()
        //        {
        //            foreach (RemotePeer peer in _connectedUsers)
        //            {
        //                peer.Disconnect();
        //            }
        //        }
    }
}